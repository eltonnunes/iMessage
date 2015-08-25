using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Server.Models.Mapping
{
    public class tbExecucaoLogMap : EntityTypeConfiguration<tbExecucaoLog>
    {
        public tbExecucaoLogMap()
        {
            // Primary Key
            this.HasKey(t => t.cdExecucaoLog);

            // Properties
            // Table & Column Mappings
            this.ToTable("tbExecucaoLog", "card");
            this.Property(t => t.cdExecucaoLog).HasColumnName("cdExecucaoLog");
            this.Property(t => t.cdLoginAdquirenteEmpresa).HasColumnName("cdLoginAdquirenteEmpresa");
            this.Property(t => t.dtTransacaoFiltroInicio).HasColumnName("dtTransacaoFiltroInicio");
            this.Property(t => t.dtTransacaoFiltroFinal).HasColumnName("dtTransacaoFiltroFinal");
            this.Property(t => t.qtTransacao).HasColumnName("qtTransacao");
            this.Property(t => t.vlTransacaoTotal).HasColumnName("vlTransacaoTotal");
            this.Property(t => t.stExecucao).HasColumnName("stExecucao");
            this.Property(t => t.dtExecucaoInicio).HasColumnName("dtExecucaoInicio");
            this.Property(t => t.dtExecucaoFim).HasColumnName("dtExecucaoFim");
            this.Property(t => t.dtExecucaoProxima).HasColumnName("dtExecucaoProxima");
            this.Property(t => t.idLogExecution).HasColumnName("idLogExecution");

            // Relationships
            this.HasRequired(t => t.tbLoginAdquirenteEmpresa)
                .WithMany(t => t.tbExecucaoLogs)
                .HasForeignKey(d => d.cdLoginAdquirenteEmpresa);

        }
    }
}
