using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Server.Models.Mapping
{
    public class BandeirasTefMap : EntityTypeConfiguration<BandeirasTef>
    {
        public BandeirasTefMap()
        {
            // Primary Key
            this.HasKey(t => new { t.IdBandeira, t.DescricaoBandeira, t.IdGrupo, t.CodBandeiraHostPagamento });

            // Properties
            this.Property(t => t.IdBandeira)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.DescricaoBandeira)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.IdGrupo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CodBandeiraERP)
                .HasMaxLength(255);

            this.Property(t => t.CodBandeiraHostPagamento)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Sacado)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("BandeirasTef", "cartao");
            this.Property(t => t.IdBandeira).HasColumnName("IdBandeira");
            this.Property(t => t.DescricaoBandeira).HasColumnName("DescricaoBandeira");
            this.Property(t => t.IdGrupo).HasColumnName("IdGrupo");
            this.Property(t => t.CodBandeiraERP).HasColumnName("CodBandeiraERP");
            this.Property(t => t.CodBandeiraHostPagamento).HasColumnName("CodBandeiraHostPagamento");
            this.Property(t => t.TaxaAdministracao).HasColumnName("TaxaAdministracao");
            this.Property(t => t.IdTipoPagamento).HasColumnName("IdTipoPagamento");
            this.Property(t => t.Sacado).HasColumnName("Sacado");
        }
    }
}
