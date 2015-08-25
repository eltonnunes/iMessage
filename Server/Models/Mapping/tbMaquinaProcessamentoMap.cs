using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Server.Models.Mapping
{
    public class tbMaquinaProcessamentoMap : EntityTypeConfiguration<tbMaquinaProcessamento>
    {
        public tbMaquinaProcessamentoMap()
        {
            // Primary Key
            this.HasKey(t => new { t.idLoginOperadora, t.ip });

            // Properties
            this.Property(t => t.idLoginOperadora)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ip)
                .IsRequired()
                .HasMaxLength(16);

            // Table & Column Mappings
            this.ToTable("tbMaquinaProcessamento", "pos");
            this.Property(t => t.idLoginOperadora).HasColumnName("idLoginOperadora");
            this.Property(t => t.ip).HasColumnName("ip");

            // Relationships
            this.HasRequired(t => t.LoginOperadora)
                .WithMany(t => t.tbMaquinaProcessamentoes)
                .HasForeignKey(d => d.idLoginOperadora);

        }
    }
}
