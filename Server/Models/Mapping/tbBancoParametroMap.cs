using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Server.Models.Mapping
{
    public class tbBancoParametroMap : EntityTypeConfiguration<tbBancoParametro>
    {
        public tbBancoParametroMap()
        {
            // Primary Key
            this.HasKey(t => new { t.cdBanco, t.dsMemo });

            // Properties
            this.Property(t => t.cdBanco)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.dsMemo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.dsTipo)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("tbBancoParametro", "card");
            this.Property(t => t.cdBanco).HasColumnName("cdBanco");
            this.Property(t => t.dsMemo).HasColumnName("dsMemo");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.dsTipo).HasColumnName("dsTipo");

            // Relationships
            this.HasOptional(t => t.tbAdquirente)
                .WithMany(t => t.tbBancoParametroes)
                .HasForeignKey(d => d.cdAdquirente);

        }
    }
}