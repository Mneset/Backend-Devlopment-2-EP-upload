export interface ClinicResponseDto {
  id: number;
  name: string;
  isActive: boolean;
}

export interface CreateClinicRequestDto {
  name: string;
}

export interface UpdateClinicRequestDto {
  id: number;
  name: string;
}
