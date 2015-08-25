using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Server.Models.Mapping
{
    public class tbEstadoTransacaoTef1Map : EntityTypeConfiguration<tbEstadoTransacaoTef1>
    {
        public tbEstadoTransacaoTef1Map()
        {
            // Primary Key
            this.HasKey(t => t.cdEstadoTransacaoTef);

            // Properties
            this.Property(t => t.cdEstadoTransacaoTef)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.dsEstadoTransacaoTef)
                .HasMaxLength(40);

            // Table & Column Mappings
            this.ToTable("tbEstadoTransacaoTef");
            this.Property(t => t.cdEstadoTransacaoTef).HasColumnName("cdEstadoTransacaoTef");
            this.Property(t => t.dsEstadoTransacaoTef).HasColumnName("dsEstadoTransacaoTef");
        }
    }
}
