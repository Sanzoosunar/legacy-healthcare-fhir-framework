using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Common;
using LegacyHealthcareFHIR.Core.Models.Legacy;
using LegacyHealthcareFHIR.Core.Models.Mapping;
using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Mapping;

public class PatientMappingService
{
    //public ProcessResult<List<PatientData>, MappingError> Map(
    //    List<LegacyRecord> records,
    //    List<FieldMapping> mappingFields)
    //{
    //    var patients = new List<PatientData>();
    //    var errors = new List<MappingError>();

    //    foreach (var record in records)
    //    {
    //        var patient = new PatientData();

    //        foreach (var mapping in mappingFields)
    //        {
    //            if (!record.Fields.TryGetValue(
    //                    mapping.SourceField,
    //                    out var value))
    //            {
    //                errors.Add(new MappingError
    //                {
    //                    ErrorType = MappingErrorType.SourceFieldNotFound,
    //                    SourceField = mapping.SourceField,
    //                    TargetField = mapping.TargetField
    //                });

    //                continue;
    //            }

    //            switch (mapping.TargetField)
    //            {
    //                case nameof(PatientData.PatientId):
    //                    patient.PatientId = value;
    //                    break;

    //                case nameof(PatientData.FirstName):
    //                    patient.FirstName = value;
    //                    break;

    //                case nameof(PatientData.LastName):
    //                    patient.LastName = value;
    //                    break;

    //                case nameof(PatientData.DateOfBirth):
    //                    patient.DateOfBirth = value;
    //                    break;

    //                case nameof(PatientData.Gender):
    //                    patient.Gender = value;
    //                    break;

    //                default:
    //                    errors.Add(new MappingError
    //                    {
    //                        ErrorType = MappingErrorType.TargetFieldNotSupported,
    //                        SourceField = mapping.SourceField,
    //                        TargetField = mapping.TargetField
    //                    });
    //                    break;
    //            }
    //        }

    //        patients.Add(patient);
    //    }

    //    return new ProcessResult<List<PatientData>, MappingError>
    //    {
    //        IsSuccess = errors.Count == 0,
    //        Data = patients,
    //        Errors = errors
    //    };
    //}
}