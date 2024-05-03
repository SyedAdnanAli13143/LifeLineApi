# LifeLineApi
Application Flow :
1) We have a landing page where all of the information about our services is provided. For 
example, we assist users without requiring any login information, and they can search for 
services to find the closest hospital to their home, saving them time and avoiding the need to visit 
multiple hospitals.
2) We also offer a chatbot that assists customers with their decisions, advises them to avoid 
certain things, or suggests that they see a doctor.
3) We also give the user the ability to quickly ascertain which hospital is offering blood for a 
given blood group by searching for it, or by calculating the probability of receiving blood by 
displaying a graph containing hospital details.
4) The customer has access to ratings and evaluations of doctors from various hospitals, enabling 
him to schedule an appointment with the highest rated physician.
5) In addition, we offer a form that allows users to easily schedule appointments with any 
hospital's doctors as well as video calls.
6) Following a successful appointment, the employee's roles begin here. When a user selects a 
hospital for their appointment, the employee receives access to the appointment details. Each 
hospital employee has an employee dashboard, and only pertinent hospital data is shared with 
them. If an employee accepts an appointment, an email containing login credentials is sent to the 
patient, and all patient dashboard details are also sent to the doctor.
7) The employee can also manage blood availability, accept or reject appointments, and add 
patients. 
8) The doctor may now add prescriptions and video chat with patients
9) the patient's history and other notes can be viewed.
10) the patient has a dynamic medical portfolio that will eliminate or minimize the need for 
paper records.
11) Patient has access to his dashboard for scheduling appointments as well as his appointment hi
story.
Because we have numerous hospitals, all of the data is centralized and shown according to relevan
ce.
12) The hospital may expand its staff, physicians, and services.
13)Therefore, the last super admin handled everything, even adding the hospital. 
14) we notify users via email and SMS before to their appointments. 
15) We also give users reminders for their medications.








========================================================
How to implement 

1)Download all project 
2)change server name and twilio configuration details in appsetting.json
3)Run below command in Tools->package manager console
Scaffold-DbContext "Server=yourserver;Database=LifeLinedb;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force


