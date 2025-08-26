using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogisticsSales.Models;

public partial class LOG_Unit
{
    [Key]
    public int Id { get; set; }

    public string UnitNameAR { get; set; } = null!;

    public string UnitNameEn { get; set; } = null!;
}
