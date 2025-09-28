using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Client
{
    [Key]
    public string Name { get; set; }

    [Required, StringLength(30)]
    public string Surname
}