using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Server.Models.Mapping
{
    public class RecebimentoParcelaMap : EntityTypeConfiguration<RecebimentoParcela>
    {
        public RecebimentoParcelaMap()
        {
            // Primary Key
            this.HasKey(t => new { t.idRecebimento, t.numParcela, t.valorParcelaBruta, t.dtaRecebimento, t.valorDescontado });

            // Properties
            this.Property(t => t.idRecebimento)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.numParcela)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.valorParcelaBruta)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.valorDescontado)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("RecebimentoParcela", "pos");
            this.Property(t => t.idRecebimento).HasColumnName("idRecebimento");
            this.Property(t => t.numParcela).HasColumnName("numParcela");
            this.Property(t => t.valorParcelaBruta).HasColumnName("valorParcelaBruta");
            this.Property(t => t.valorParcelaLiquida).HasColumnName("valorParcelaLiquida");
            this.Property(t => t.dtaRecebimento).HasColumnName("dtaRecebimento");
            this.Property(t => t.valorDescontado).HasColumnName("valorDescontado");
        }
    }
}
