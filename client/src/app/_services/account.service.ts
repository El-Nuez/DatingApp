import { HttpClient } from '@angular/common/http';
import { inject , Injectable, signal } from '@angular/core';
import { map, Observable } from 'rxjs';
import { User } from '../_models/user';
import { environment } from '../../environments/environment';
import { LikesService } from './likes.service';


@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private  http = inject (HttpClient);
  private  likesServices = inject (LikesService);
  baseUrl = environment.apiUrl;
  currentUser = signal <User | null>(null);

  login(model : any): Observable<User |void > {
    return this.http.post<User>(this.baseUrl + "account/login" , model).pipe(
      map(user => {
        if (user){
          this.setCurrentUser(user);
        }
      })
    );
  }

  register(model : any): Observable<User |void > {
    return this.http.post<User>(this.baseUrl + "account/register" , model).pipe(
      map(user => {
        if (user){
         this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  setCurrentUser(user: User): void {
    localStorage.setItem("user", JSON.stringify(user));
    this.currentUser.set(user)
    this.likesServices.getLikeIds();
  }


  logout(): void {
    localStorage.removeItem("user");
    this.currentUser.set(null);
  }

}
