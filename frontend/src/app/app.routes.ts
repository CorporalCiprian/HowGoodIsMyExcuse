import { Routes } from '@angular/router';
import { SigninComponent } from './signin/signin.component';
import { RegisterComponent } from './register/register.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { SubmitExcuseComponent } from './submit-excuse/submit-excuse.component';
import { MyExcusesComponent } from './my-excuses/my-excuses.component';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'login', component: SigninComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'submit-excuse', component: SubmitExcuseComponent },
  { path: 'my-excuses', component: MyExcusesComponent },
];