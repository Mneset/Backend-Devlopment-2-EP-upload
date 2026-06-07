export type Status = "Scheduled" | "Completed" | "Cancelled";

export interface RescheduleAppointmentRequestDto {
  id: number;
  appointmentDate: string;
  durationInMin: number;
}

export interface AppointmentResponseDto {
  id: number;
  status: Status;
  appointmentDate: string;
  durationInMin: number;
  patientId: number;
  patientFirstName: string;
  patientLastName: string;
  doctorId: number;
  doctorFirstName: string;
  doctorLastName: string;
  clinicId: number;
  clinicName: string;
  appointmentCategoryId: number;
  appointmentCategoryName: string;
}
