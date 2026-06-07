export type Gender = "Male" | "Female";

export type Religion =
  | "None"
  | "Christianity"
  | "Islam"
  | "Judaism"
  | "Hinduism"
  | "Buddhism"
  | "Other"
  | "PreferNotToSay";

export interface RegisterPatientRequestDto {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  gender: Gender;
  dateOfBirth: string;
  religion: Religion;
  socialSecurityNumber: string;
  taxNumber: string;
  driverLicenseNumber: string;
  medicalInsuranceNumber: string;
}

export interface MyProfileResponseDto {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  gender: Gender;
  dateOfBirth: string;
  religion: Religion | null;
  socialSecurityNumber: string | null;
  taxNumber: string | null;
  driverLicenseNumber: string | null;
  medicalInsuranceNumber: string | null;
}

export interface UpdateOwnProfileRequestDto {
  email: string;
  firstName: string;
  lastName: string;
  gender: Gender;
  dateOfBirth: string;
  religion: Religion | null;
}

export interface PatientResponseDto {
  id: number;
  email: string | null;
  firstName: string;
  lastName: string;
  gender: Gender;
  dateOfBirth: string;
  isActive: boolean;
  isRegistered: boolean;
  religion: Religion | null;
  socialSecurityNumber: string | null;
  taxNumber: string | null;
  driverLicenseNumber: string | null;
  medicalInsuranceNumber: string | null;
}

export interface UpdatePatientRequestDto {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  gender: Gender;
  dateOfBirth: string;
  religion: Religion | null;
  socialSecurityNumber: string | null;
  taxNumber: string | null;
  driverLicenseNumber: string | null;
  medicalInsuranceNumber: string | null;
}
