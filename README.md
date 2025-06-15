# Query Runner System

## Overview

The **Query Runner** system is a tool designed to execute SQL queries across multiple target databases simultaneously and manage their execution results efficiently. 

---

## Key Features

- **Multi-Database Query Execution:** Run SQL queries on multiple databases at once.
- **Batch Query Processing:** Execute all queries stored in a specified folder automatically.
- **Execution Logs:** Save detailed logs of execution results and errors into files for auditing and troubleshooting.
- **Error Handling:** Detect and log incorrect or failed queries, preventing them from disrupting the batch.
- **Selective Query Satisfaction:** Identify which queries were satisfied in which databases and logs that too.

---

## System Workflow

### 1. Batch Query Execution
- Reads all SQL files from a designated folder.
- Executes each query on all configured databases.
- Logs both success and failure results.


### 3. Error Handling
- Catches queries with syntax or logic issues.
- Logs them into a file such as `myapp.txt`.
- Faulty queries do not interrupt the full batch process.

### 4. Partial Query Satisfaction
- Some queries may only be valid for specific databases.
- The system logs which queries succeeded and where.

---

## How to Use

1. **Prepare your SQL Queries**
   - Place `.sql` files in the designated folder.

2. **Configure Databases**
   - Set up connection strings for each target database
     
3. **Run the Query Runner**
   - Execute the system to process all queries across the configured databases.

4. **Review Logs**
