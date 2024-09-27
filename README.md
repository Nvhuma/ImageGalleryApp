# **Image Gallery App**

## **Overview**
The Image Gallery App is a full-stack web application that allows users to upload, view, manage, and interact with images. The app features:
- Role-based user management
- Two-Factor Authentication (TOTP)
- Image upload, view, update, and delete functionality
- Pagination and frontend validations for enhanced user experience
- API endpoints for developers to interact with the image library

The application architecture includes a **React** frontend, a **.NET Core** backend, and **SQL Server** as the database.

---

## **Table of Contents**
1. [Installation](#installation)
2. [Features](#features)
3. [API Documentation](#api-documentation)
4. [Project Structure](#project-structure)
5. [Technologies Used](#technologies-used)
6. [Running the Application](#running-the-application)
7. [Development Setup](#development-setup)
8. [Bonus Features](#bonus-features)



---

## **Installation**

### **Frontend (React)**
1. Clone the repository:
   ```bash
   git clone https://https://github.com/Nvhuma/ImageGalleryApp.git
   cd ImageGalleryAPP


Navigate to the frontend directory and install dependencies:

bash

cd frontend
npm install
Start the development server:

bash

npm start
The app will run at http://localhost:5173.

Backend (.NET Core API)
Navigate to the backend directory and configure the database connection string in appsettings.json:

json

{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SINGLRN-NHLAYIS\\NBVUMA;Database=ImageGalleryApp;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
  }
}
Run the backend API:


bash
cd ImageGalleryApp/api
dotnet run
The API will run at http://localhost:5263.


## *Features*

*User Authentication: Register, login, and manage accounts with JWT-based authentication.*


TOTP (Two-Factor Authentication): Adds an extra layer of security by requiring a TOTP code during login.


Image Upload: Allows users to upload images with metadata (title, description).


Image Management: View, update, and delete images, with permissions restricted to the image owner.


Pagination: Enhances performance by loading images in pages for users with large libraries.


Role-Based Access: Ensures only the rightful user can access their images and manage their library.



## **API Documentation**
User Endpoints
Register:
POST /api/Account/register
Request Body:

json

{
  "userName": "string",
  "names": "string",
  "emailAddress": "user@example.com",
  "password": "string"
}
## **Login:**
POST /api/Account/login
Request Body:

json
{
  "userName": "string",
  "password": "string"
}

## **TOTP Authentication:**
POST /api/Account/totp
Request Body:

json
{
  "userName": "string",
  "password": "string",
  "totpCode": "123456"
}
## **Forgot Password:**
POST /api/Account/forgot-password
Request Body:


json

{
  "email": "user@example.com"
}

## **Reset Password:**
POST /api/Account/reset-password
Request Body:


json
{
  "ResetToken": "string",
  "email": "user@example.com",
  "newPassword": "newpassword123",
  "confirmPassword": "newpassword123"
}



## **Image Endpoints**
## **Get All Images:**
GET /api/image

## **Create an Image:**
POST /api/image
Request Body:


json
{
  "title": "Image Title",
  "url": "https://example.com/newimage.jpg",
  "description": "Description of the image",
  "CreateDate": "2024-09-26"
}

## **Update an Image:**
PUT /api/image/{id}
Request Body:


json
{
  "url": "https://example.com/updatedimage.jpg",
  "description": "Updated description of the image"
}

## **Delete an Image:**
DELETE /api/image/{id}

## **Get User's Library:**
GET /api/image/mylibrary


## **Project Structure**
image-gallery-app/
 ├── public/                  # Static assets
├── src/
│   ├── components/           # React components
│   ├── services/             # API service logic
│   ├── pages/                # App pages (Home, Upload, My Library)
│   ├── App.js                # Main app component
│   └── index.js              # Entry point for React
├── .env                      # Environment variables for API keys and backend URL
├── package.json              # Project dependencies
└── README.md                 # Project README


## **Technologies Used**

Frontend: React, Axios for API calls

Backend: .NET Core, Entity Framework Core

Database: SQL Server

Authentication: JWT (JSON Web Token)

State Management: React Context API / Redux

API Testing: Swagger





## **Running the Application**
Frontend (React)
Navigate to the frontend directory and install the dependencies:

bash
npm install
Run the React app:

bash
npm start
The app will be available at http://localhost:5173.

Backend (.NET Core API)
Navigate to the backend directory and run the API:

bash
dotnet run
The API will run at http://localhost:5263.

## **Bonus Features**
Verification Email:
Users receive a verification email upon registration.

## **Pagination:**
Images are loaded in pages to enhance performance, especially for users with many images.





