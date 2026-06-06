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
