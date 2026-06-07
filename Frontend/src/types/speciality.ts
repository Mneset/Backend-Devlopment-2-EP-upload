export interface SpecialityResponseDto {
  id: number;
  name: string;
}

export interface CreateSpecialityRequestDto {
  name: string;
}

export interface UpdateSpecialityRequestDto {
  id: number;
  name: string;
}
