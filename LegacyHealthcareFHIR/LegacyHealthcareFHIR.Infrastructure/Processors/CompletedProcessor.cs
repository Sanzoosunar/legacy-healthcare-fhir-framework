using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Validation;

namespace LegacyHealthcareFHIR.Infrastructure.Processors;
public class CompletedProcessor:IJobProcessor
{
    private readonly IJobRepository _jobRepo;
    private readonly ISignalRNotifier _signalRNotifier;
    private readonly LocalFileStorageService _fileStorage;

    public JobStage _currentStage => JobStage.Completed;

    public CompletedProcessor(IJobRepository jobRepo, LocalFileStorageService fileStorage, ISignalRNotifier signalRNotifier)
    {
        _jobRepo = jobRepo;
        _fileStorage = fileStorage;
        _signalRNotifier = signalRNotifier;
    }

    public void ValidateJob(ImportJob job)
    {
        if(job.OutputFileName == null) throw new Exception("Missing outpur file");
        var isValidState = JobStateValidator.IsValid(job.JobStage, job.Status, _currentStage);

        if (!isValidState) throw new Exception("Not valid stage or status");
    }
    public async Task ExecuteAsync(Guid jobId)
    {
        var job = await _jobRepo.GetAsync(jobId);

        ValidateJob(job);

        try
        {
            var savedFileName = job.StoredFileName;
            await _fileStorage.Remove(savedFileName);

            job.Status = JobStatus.Completed;
            job.JobStage = _currentStage;
            job.StoredFileName = string.Empty;
            await _jobRepo.UpdateAsync(job);

            await _signalRNotifier.SendAsync(job.Id, job.JobStage, job.Status,"Completed");
        }
        catch(Exception ex)
        {
            job.Status = JobStatus.Failed;
            job.JobStage = _currentStage;
            await _jobRepo.UpdateAsync(job);

            await _signalRNotifier.SendAsync(job.Id, job.JobStage, job.Status, ex.Message);
        }
    }
}