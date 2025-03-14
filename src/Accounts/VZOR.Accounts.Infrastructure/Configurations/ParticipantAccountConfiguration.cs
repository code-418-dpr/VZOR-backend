﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VZOR.Accounts.Domain;

namespace VZOR.Accounts.Infrastructure.Configurations;

public class ParticipantAccountConfiguration: IEntityTypeConfiguration<ParticipantAccount>
{
    public void Configure(EntityTypeBuilder<ParticipantAccount> builder)
    {
        builder.ToTable("participant_accounts");
        
        builder.HasOne(pa => pa.User)
            .WithOne(u => u.ParticipantAccount)
            .HasForeignKey<ParticipantAccount>(pa => pa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasKey(pa => pa.Id);
    }
}