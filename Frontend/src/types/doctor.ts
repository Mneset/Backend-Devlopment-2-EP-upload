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

export interface CreateDoctorRequestDto {
  firstName: string;
  lastName: string;
  gender: Gender;
  specialityId: number;
  clinicId: number;
}

export interface UpdateDoctorRequestDto {
  id: number;
  firstName: string;
  lastName: string;
  gender: Gender;
  specialityId: number;
  clinicId: number;
}
