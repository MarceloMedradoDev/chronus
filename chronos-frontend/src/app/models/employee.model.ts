  export interface EmployeeReturnDTO {
    role: string;
    totalHours: string;
    isCompleteDay: boolean;
  }
  
  export interface ResponseModel<T> {
    success: boolean;
    message: string;
    data: T | null;
  }

  export interface EmployeeModel {
    id: number;
    registration?: number;
    name: string;
    role?: string;
    totalHours: string;
    isCompleteDay: boolean;
    day: Date;
  }
  