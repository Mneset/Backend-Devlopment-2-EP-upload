export interface AppointmentCategoryResponseDto {
  id: number;
  name: string;
}

export interface CreateAppointmentCategoryRequestDto {
  name: string;
}

export interface UpdateAppointmentCategoryRequestDto {
  id: number;
  name: string;
}
