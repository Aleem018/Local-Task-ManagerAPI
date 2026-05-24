using System;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;
using LocalTaskManager.Models;

namespace LocalTaskManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    // my GET method
    [HttpGet]
    public IActionResult GetAllTasks()
    {
        // To get all running processes on windows
        Process[] processList = Process.GetProcesses();

        var list = new List<SystemTask> {};

        foreach (var process in processList)
        {
            var myTask = new SystemTask();

            myTask.ProcessId = process.Id;
            myTask.ProcessName = process.ProcessName;

            list.Add(myTask);
        }

        return Ok(list);
    }

    // To map a new endpoint that gives out a specific process
    [HttpGet("{id}")]
    public IActionResult GetTaskById(int id)
    {
        try
        {
            Process process = Process.GetProcessById(id);

            var newTask = new SystemTask();

            newTask.ProcessId = process.Id;
            newTask.ProcessName = process.ProcessName;

            return Ok(newTask);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    // My Delete method
    [HttpDelete("{id}")]
    public IActionResult KillTask(int id)
    {
        try
        {
            Process process = Process.GetProcessById(id);


            process.Kill();

            return Ok($"Process {process.ProcessName} has been terminated.");

        }
        catch (ArgumentException) // If the ID doesn't exist
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500, "Access Denied: You do not have permission to kill this task.");
        }
    }

    [HttpPost]
    public IActionResult LaunchTask([FromBody] TaskLaunchRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ProgramName))
            {
                return BadRequest("Program name cannot be empty.");
            }

            Process newProcess = Process.Start(request.ProgramName);

            if (newProcess != null)
            {
                return Ok($"Successfully launched {request.ProgramName} with ID: {newProcess.Id}");
            }

            return Ok($"Successfully launched {request.ProgramName}.");
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return NotFound($"Windows could not find a program named '{request.ProgramName}'.");
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, $"An error occured while launching the program: {ex.Message}");
        }
    }
}