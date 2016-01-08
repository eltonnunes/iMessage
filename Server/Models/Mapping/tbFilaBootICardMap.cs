using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Mapping
{
    public class tbFilaBootICardMap : EntityTypeConfiguration<tbFilaBootICard>
    {
        public tbFilaBootICardMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.cdAdquirente)
                .IsRequired();

            this.Property(t => t.nrCNPJ)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(14);

            this.Property(t => t.dsModalidade)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.dtInicio)
                .IsRequired();

            this.Property(t => t.dtFim)
                .IsRequired();

            this.Property(t => t.stProcessamento)
                .IsRequired();

            this.Property(t => t.dtInsert)
                .IsRequired();

            this.Property(t => t.cdUserInsert)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tbFilaBootICard", "card");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.dsModalidade).HasColumnName("dsModalidade");
            this.Property(t => t.dtInicio).HasColumnName("dtInicio");
            this.Property(t => t.dtFim).HasColumnName("dtFim");
            this.Property(t => t.stProcessamento).HasColumnName("stProcessamento");
            this.Property(t => t.cdUser).HasColumnName("cdUser");
            this.Property(t => t.dtProcessamento).HasColumnName("dtProcessamento");
            this.Property(t => t.dtInsert).HasColumnName("dtInsert");
            this.Property(t => t.cdUserInsert).HasColumnName("cdUserInsert");

            // Relationships
            this.HasRequired(t => t.empresa)
                .WithMany(t => t.tbFilaBootICards)
                .HasForeignKey(d => d.nrCNPJ);
            this.HasRequired(t => t.tbAdquirente)
                .WithMany(t => t.tbFilaBootICards)
                .HasForeignKey(d => d.cdAdquirente);
            this.HasOptional(t => t.webpages_Users)
                .WithMany(t => t.tbFilaBootICards)
                .HasForeignKey(d => d.cdUser);
            this.HasRequired(t => t.webpages_UsersInsert)
                .WithMany(t => t.tbFilaBootICardInserts)
                .HasForeignKey(d => d.cdUserInsert);
        }
    }
}
