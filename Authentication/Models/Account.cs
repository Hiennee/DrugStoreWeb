﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Authentication.Models;

public partial class Account
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string FullName { get; set; }

    public string RoleId { get; set; }

    public string Password { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}