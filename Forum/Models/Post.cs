﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Forum.Models;

public partial class Post
{
    public int PostId { get; set; }

    public int PosterId { get; set; }

    public int? PredecessorId { get; set; }

    public string Text { get; set; }

    public virtual ICollection<Post> InversePredecessor { get; set; } = new List<Post>();

    public virtual User Poster { get; set; }

    public virtual Post Predecessor { get; set; }

    public virtual ICollection<Thread> Thread { get; set; } = new List<Thread>();
}