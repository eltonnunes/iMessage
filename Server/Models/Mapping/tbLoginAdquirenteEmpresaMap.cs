using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Server.Models.Mapping
{
    public class tbLoginAdquirenteEmpresaMap : EntityTypeConfiguration<tbLoginAdquirenteEmpresa>
    {
        public tbLoginAdquirenteEmpresaMap()
        {
            // Primary Key
            this.HasKey(t => t.cdLoginAdquirenteEmpresa);

            // Properties
            this.Property(t => t.nrCNPJ)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(14);

            this.Property(t => t.dsLogin)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.dsSenha)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.cdEstabelecimento)
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("tbLoginAdquirenteEmpresa", "card");
            this.Property(t => t.cdLoginAdquirenteEmpresa).HasColumnName("cdLoginAdquirenteEmpresa");
            this.Property(t => t.cdAdquirente).HasColumnName("cdAdquirente");
            this.Property(t => t.cdGrupo).HasColumnName("cdGrupo");
            this.Property(t => t.nrCNPJ).HasColumnName("nrCNPJ");
            this.Property(t => t.dsLogin).HasColumnName("dsLogin");
            this.Property(t => t.dsSenha).HasColumnName("dsSenha");
            this.Property(t => t.cdEstabelecimento).HasColumnName("cdEstabelecimento");
            this.Property(t => t.dtAlteracao).HasColumnName("dtAlteracao");
            this.Property(t => t.stLoginAdquirente).HasColumnName("stLoginAdquirente");
            this.Property(t => t.stLoginAdquirenteEmpresa).HasColumnName("stLoginAdquirenteEmpresa");

            // Relationships
            this.HasRequired(t => t.tbAdquirente)
                .WithMany(t => t.tbLoginAdquirenteEmpresas)
                .HasForeignKey(d => d.cdAdquirente);
            this.HasRequired(t => t.empresa)
                .WithMany(t => t.tbLoginAdquirenteEmpresas)
                .HasForeignKey(d => d.nrCNPJ);

        }
    }
}
