using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Server.Models.Mapping
{
    public class LogNotaDaGenteMap : EntityTypeConfiguration<LogNotaDaGente>
    {
        public LogNotaDaGenteMap()
        {
            // Primary Key
            this.HasKey(t => t.id_LogNotaDaGente);

            // Properties
            this.Property(t => t.ds_observacao)
                .HasMaxLength(1000);

            this.Property(t => t.nu_cnpj)
                .IsFixedLength()
                .HasMaxLength(14);

            // Table & Column Mappings
            this.ToTable("LogNotaDaGente", "notadagente");
            this.Property(t => t.id_LogNotaDaGente).HasColumnName("id_LogNotaDaGente");
            this.Property(t => t.nu_pdv).HasColumnName("nu_pdv");
            this.Property(t => t.nu_protocolo).HasColumnName("nu_protocolo");
            this.Property(t => t.ds_observacao).HasColumnName("ds_observacao");
            this.Property(t => t.dt_movimento).HasColumnName("dt_movimento");
            this.Property(t => t.dt_envio).HasColumnName("dt_envio");
            this.Property(t => t.fl_status).HasColumnName("fl_status");
            this.Property(t => t.nu_cnpj).HasColumnName("nu_cnpj");
        }
    }
}
