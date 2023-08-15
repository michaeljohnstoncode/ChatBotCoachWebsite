using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ChatBotCoachWebsite.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ChatBotCoachWebsiteUser class
public class ChatBotCoachWebsiteUser : IdentityUser
{
    
    [PersonalData]
    [Column(TypeName = "nvarchar(100)"), AllowNull]
    public string? FirstName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)"), AllowNull]
    public string? LastName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)"), AllowNull]
    public string? Gamertag { get; set; }
    
}

