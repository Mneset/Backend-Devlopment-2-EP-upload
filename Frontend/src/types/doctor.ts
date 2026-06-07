import type { Gender } from "./patient";

export interface DoctorResponseDto {
  id: number;
  firstName: string;
  lastName: string;
  gender: Gender;
  isActive: boolean;
  specialityId: number;
  specialityName: string;
  clinicId: number;
  clinicName: string;
}
