# JSON File Comparator

## Project Overview

This project is an F# application designed to compare and validate JSON files. It allows users to identify differences between two JSON files, validate their structure, and sort their contents for easier analysis.

## Features

1. **JSON Validation**:
    - Ensures the syntax of JSON files is correct.
    - Provides meaningful error messages for invalid JSON files.

2. **JSON Sorting**:
    - Recursively sorts JSON objects and arrays alphabetically by keys.
    - Handles nested structures efficiently.

3. **JSON Comparison**:
    - Compares two JSON files recursively.
    - Identifies differences in keys, values, and array sizes.
    - Outputs differences in a structured format (string list).

4. **Customizable Output**:
    - Differences are presented line-by-line for better readability.
    - Supports detailed reporting of discrepancies.

## File Structure

The project contains the following files:

- **Json.fs**: Contains functions for reading, validating, and sorting JSON files.
- **json2.fs**: Implements recursive comparison logic for JSON files.
- **Program.fs**: Entry point of the application; integrates sorting and comparison functionalities.
- **a.json & b.json**: Example JSON files used for testing.

## How It Works

1. **Validation**:
   The `readJsonFile` function reads and parses JSON files using `System.Text.Json`. Invalid files result in descriptive error messages.

2. **Sorting**:
   The `sortJsonProperties` function recursively sorts objects and arrays by their keys.

3. **Comparison**:
   The `compareJson` function compares two sorted JSON structures recursively, identifying differences in keys, values, and array sizes.

4. **Output**:
   The `compareJsonFiles` function returns a list of discrepancies or a message indicating that the files are identical.

## Dependencies

- **System.Text.Json**: For parsing and handling JSON data.
- **F# Core Libraries**: Functional programming utilities.

## Usage

### Installation
1. Clone the repository:
