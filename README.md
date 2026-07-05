````markdown
# Mobile_AR_Tutorial

A mobile Augmented Reality (AR) application developed as part of the master's dissertation:

**Exploring Augmented Reality in Industry 5.0: Impact and Usability of Tools for Creating Tutorials**

The purpose of this application is to enable users with no prior knowledge of programming, 3D modeling, or Augmented Reality application development to create, edit, and visualize interactive tutorials directly on Android mobile devices.

---

# Overview

The application uses marker-based Augmented Reality (*Image Targets*) to position virtual objects on real-world equipment. These objects are used to guide users during operational procedures, maintenance activities, training sessions, and knowledge transfer processes.

The system supports two primary user profiles:

* **Tutorial Creators:** responsible for creating and editing instructions.
* **End Users:** responsible for viewing and executing procedures.

---

# Main Features

## Tutorial Management

* Create tutorials
* Edit tutorials
* Rename tutorials
* Delete tutorials
* Filter tutorials

## Task Management

* Create tasks
* Edit tasks
* Delete tasks
* Reorder tasks
* Preview tasks

## Virtual Objects

The application supports the following virtual elements:

* Directional Arrow
* Clockwise Arrow
* Counterclockwise Arrow
* 3D Text
* Attention Indicator
* Correct Indicator
* Incorrect Indicator
* Prohibition Indicator
* Virtual Tools

Each object can be individually configured through the following properties:

* Position
* Rotation
* Scale
* Color
* Associated Text

---

# Architecture

```text
User
   │
   ▼
Android Application
(Unity + Vuforia)
   │
   ├── Database
   │      (MongoDB Atlas)
   │
   └── Object Repository
          (Asset Bundles)
```

The database is responsible for storing:

* Tutorials
* Tasks
* Virtual Objects
* Object Configurations

The virtual object repository stores the models and assets used by the application.

---

# Technologies Used

| Technology     | Purpose                             |
| -------------- | ----------------------------------- |
| Unity          | Application Development             |
| C#             | Programming Language                |
| Vuforia Engine | Augmented Reality Tracking          |
| MongoDB Atlas  | Database                            |
| Android        | Mobile Platform                     |
| Blender        | 3D Modeling                         |
| Figma          | User Interface Prototyping          |
| Git            | Version Control                     |

---

# Project Structure

```text
Mobile_AR_Tutorial
│
├── Assets
├── Packages
├── ProjectSettings
├── UserSettings
└── ...
```

The most important directories are:

### Assets

Contains:

* C# Scripts
* Prefabs
* Images
* 3D Objects
* User Interfaces
* AR Resources

### Packages

Unity packages used in the project:

* Vuforia Engine
* Newtonsoft Json
* Project Dependencies

### ProjectSettings

Unity project configuration files.

---

# Database Configuration

The project uses MongoDB Atlas.

## Step 1

Create an account at:

https://www.mongodb.com/atlas

## Step 2

Create a Cluster.

## Step 3

Create a Database.

Example:

```text
ARTutorials
```

## Step 4

Create a Collection:

```text
Tutorials
```

## Step 5

Create a user with read and write permissions.

## Step 6

Configure the IP Access List.

## Step 7

Obtain the Connection String.

Example:

```text
mongodb+srv://username:password@cluster.mongodb.net/
```

## Step 8

Insert the Connection String into the scripts responsible for database connectivity.

---

# Virtual Object Repository Configuration

The application dynamically downloads the objects used in tutorials.

Asset Bundles can be stored in:

* Google Drive
* Azure Blob Storage
* AWS S3
* HTTP Server
* Internal Corporate Network
* Corporate NAS

After configuring the repository, the object URLs must be associated with the corresponding records stored in the database.

---

# Build Instructions

## Requirements

* Unity 2022 LTS or later
* Android SDK
* OpenJDK
* Android Build Support

## Generate APK

In Unity:

```text
File
→ Build Settings
→ Android
→ Switch Platform
→ Build
```

## Generate App Bundle

```text
File
→ Build Settings
→ Android
→ Build App Bundle (Google Play)
```

---

# Production Deployment

A typical deployment architecture may include:

### Database

* MongoDB Atlas

### Object Repository

* Azure Blob Storage
* AWS S3
* Corporate Server

### Clients

* Android Smartphones

In industrial environments, both the database and virtual objects can be hosted on internal company servers, ensuring that all data remains within the organization's infrastructure.

---

# Results

The application was evaluated by domain experts and end users.

Average results obtained:

| Group          | SUS   | TAM  |
| -------------- | ----- | ---- |
| Domain Experts | 85.60 | 4.47 |
| End Users      | 92.57 | 4.68 |

The results indicate high levels of usability and technology acceptance across both evaluated groups.

---

# Publication

This project resulted in the publication:

**Interactive AR Tutorials: A Simplified Tool for Industry 5.0**

SBAI 2025 – Brazilian Symposium on Intelligent Automation.

---

# Author

Rodrigo José de Paiva

Graduate Program in Instrumentation, Control, and Automation of Mining Processes (PROFICAM)

Federal University of São João del-Rei (UFSJ)

---

# License

This project is made available for academic and scientific purposes.
````
