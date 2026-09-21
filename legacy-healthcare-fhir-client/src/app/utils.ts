
// export class JobNotificationParser {
//   public static parseResourceTypeDetection(event: JobNotificationEvent): ResourceTypeDetectionResult | undefined {
//     if (event.stage !== JobStage.ResourceTypeDetection || !event.data) {
//       return undefined;
//     }

//     return event.data as ResourceTypeDetectionResult;
//   }

//   public static parseFieldMapping(event: JobNotificationEvent): FieldMappingResult | undefined {
//     if (event.stage !== JobStage.FieldMapping || !event.data) {
//       return undefined;
//     }

//     return event.data as FieldMappingResult;
//   }
// }