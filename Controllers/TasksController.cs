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
}