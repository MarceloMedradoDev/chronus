import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ResponseModel, EmployeeReturnDTO, EmployeeModel } from '../models/employee.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'http://localhost:5271/Employee';

  constructor(private http: HttpClient) {}

  // GET: /Employee/GetTotalHours
  getTotalHours(): Observable<ResponseModel<EmployeeReturnDTO>[]> {
    return this.http.get<ResponseModel<EmployeeReturnDTO>[]>(`${this.baseUrl}/get-total`);
  }

  // POST: /Employee/InsertHours
  insertHours(formData: FormData): Observable<ResponseModel<any>[]> {
    return this.http.post<ResponseModel<any>[]>(`${this.baseUrl}/insert`, formData);
  }  

  // GET: /Employee/Reports
  getReports(): Observable<ResponseModel<EmployeeModel>[]> {
    return this.http.get<ResponseModel<EmployeeModel>[]>(`${this.baseUrl}/reports`);
  }
  
  reprocessar(): Observable<string> {
  const token = localStorage.getItem('token'); 

  let headers = new HttpHeaders();
  if (token) {
    headers = headers.set('Authorization', `Bearer ${token}`);
  }

  return this.http.post<string>(`${this.baseUrl}/reprocess`, { headers });
}


}
