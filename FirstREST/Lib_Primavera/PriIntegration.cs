﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interop.ErpBS800;
using Interop.StdPlatBS800;
using Interop.StdBE800;
using Interop.GcpBE800;
using ADODB;
using Interop.IGcpBS800;
//using Interop.StdBESql800;
//using Interop.StdBSSql800;

namespace FirstREST.Lib_Primavera
{
    public class PriIntegration
    {
        

        # region Cliente

        public static List<Model.Cliente> ListaClientes()
        {
            
            
            StdBELista objList;

            List<Model.Cliente> listClientes = new List<Model.Cliente>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                //objList = PriEngine.Engine.Comercial.Clientes.LstClientes();

                objList = PriEngine.Engine.Consulta("SELECT Cliente, Nome, Moeda, NumContrib as NumContribuinte, Fac_Mor AS campo_exemplo FROM  CLIENTES");

                
                while (!objList.NoFim())
                {
                    listClientes.Add(new Model.Cliente
                    {
                        CodCliente = objList.Valor("Cliente"),
                        NomeCliente = objList.Valor("Nome"),
                        Moeda = objList.Valor("Moeda"),
                        NumContribuinte = objList.Valor("NumContribuinte"),
                        Morada = objList.Valor("campo_exemplo")
                    });
                    objList.Seguinte();

                }

                return listClientes;
            }
            else
                return null;
        }

        public static Lib_Primavera.Model.Cliente GetCliente(string codCliente)
        {
            

            GcpBECliente objCli = new GcpBECliente();


            Model.Cliente myCli = new Model.Cliente();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                if (PriEngine.Engine.Comercial.Clientes.Existe(codCliente) == true)
                {
                    objCli = PriEngine.Engine.Comercial.Clientes.Edita(codCliente);
                    myCli.CodCliente = objCli.get_Cliente();
                    myCli.NomeCliente = objCli.get_Nome();
                    myCli.Moeda = objCli.get_Moeda();
                    myCli.NumContribuinte = objCli.get_NumContribuinte();
                    myCli.Morada = objCli.get_Morada();
                    return myCli;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;
        }

        #endregion Cliente;   // -----------------------------  END   CLIENTE    -----------------------

        #region Artigo

        public static Lib_Primavera.Model.Artigo GetArtigo(string codArtigo)
        {
            
            GcpBEArtigo objArtigo = new GcpBEArtigo();
            Model.Artigo myArt = new Model.Artigo();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                if (PriEngine.Engine.Comercial.Artigos.Existe(codArtigo) == false)
                {
                    return null;
                }
                else
                {
                    objArtigo = PriEngine.Engine.Comercial.Artigos.Edita(codArtigo);
                    myArt.CodArtigo = objArtigo.get_Artigo();
                    myArt.DescArtigo = objArtigo.get_Descricao();

                    return myArt;
                }
                
            }
            else
            {
                return null;
            }

        }

        public static List<Model.Artigo> ListaArtigos()
        {
                        
            StdBELista objList;

            Model.Artigo art = new Model.Artigo();
            List<Model.Artigo> listArts = new List<Model.Artigo>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();

                while (!objList.NoFim())
                {
                    art = new Model.Artigo();
                    art.CodArtigo = objList.Valor("artigo");
                    art.DescArtigo = objList.Valor("descricao");

                    listArts.Add(art);
                    objList.Seguinte();
                }

                return listArts;

            }
            else
            {
                return null;

            }

        }

        #endregion Artigo

        #region DocsVenda

        public static Boolean Faturas_Check(string numdoc)

        {
            List<Model.DocVenda> compras = Encomendas_List();
            List<Model.DocVenda> vendas = Faturas_List();

            Model.DocVenda tempDoc = null;

            foreach (Model.DocVenda compra in compras)
            {
                if (compra.NumDoc.Equals(int.Parse(numdoc)))
                {
                    tempDoc = compra;
                    break;
                }
            }
            if (tempDoc == null) return false;

            List <Model.LinhaDocVenda> produtosCompra = tempDoc.LinhasDoc;

            foreach (Model.DocVenda fatura in vendas)
            {
                Boolean foundEqual = false;
                if(fatura.LinhasDoc.Count == produtosCompra.Count && fatura.Entidade.Equals(tempDoc.Entidade)){
                    for (int i = 0; i < fatura.LinhasDoc.Count; i++)
                    {
                        if (produtosCompra[i].CodArtigo.Equals(fatura.LinhasDoc[i].CodArtigo) && produtosCompra[i].Quantidade.Equals(fatura.LinhasDoc[i].Quantidade) && produtosCompra[i].PrecoUnitario.Equals(fatura.LinhasDoc[i].PrecoUnitario) && produtosCompra[i].TotalLiquido.Equals(fatura.LinhasDoc[i].TotalLiquido))
                            foundEqual = true;
                        else {
                            foundEqual = false;
                            break;
                        }
                    }

                    if (foundEqual)
                        return true;

                }
            }
            return false;


        }

       

        public static Model.RespostaErro Encomendas_New(Model.DocVenda dv)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            GcpBEDocumentoVenda myEnc = new GcpBEDocumentoVenda();
            GcpBEDocumentoVenda myFac = new GcpBEDocumentoVenda();
            GcpBELinhaDocumentoVenda myLin = new GcpBELinhaDocumentoVenda();
            GcpBELinhasDocumentoVenda myLinhas = new GcpBELinhasDocumentoVenda();
            PreencheRelacaoVendas rl = new PreencheRelacaoVendas();
            GcpBESerie serie = new GcpBESerie();
            List<Model.LinhaDocVenda> lstlindv = new List<Model.LinhaDocVenda>();
            try
            {
                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {
                    if (PriEngine.Engine.Comercial.Vendas.Existe("000", "ECL", "A", dv.NumDoc))
                    {
                        myEnc = PriEngine.Engine.Comercial.Vendas.Edita("000", "ECL", "A", dv.NumDoc);
                    }
                    myFac.set_NumDoc(myEnc.get_NumDoc());
                    myFac.set_Entidade(dv.Entidade);
                    myFac.set_TipoEntidade("C");
                    myFac.set_Tipodoc("FA");
                    myFac.set_Serie("D");
                    myFac.set_Assinatura(myEnc.get_Assinatura());

                    //Falta a Série do Doc Destino.Sugiro a utilização do método SeriePorDefeito mas para isso tb é necessario a data do doc destino
                    myFac.set_DataDoc(new DateTime());
                    serie.set_SeriePorDefeito(true);
                    //Usar o preenche dados relacionados do Doc Destino
                    String avisos = "";
                    PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myFac, PreencheRelacaoVendas.vdDadosTodos);
                    //PriEngine.Engine.Comercial.Vendas.Actualiza(myFac, avisos);
                    erro.Descricao += "Actualiza: " + avisos;
                    List<GcpBEDocumentoVenda> l = new List<GcpBEDocumentoVenda>();
                    l.Add(myEnc);
                    //Deve ser usado o strAvisos e passar como parametro no metodo
                    //Devem ser removidas as transações
                    String strAvisos = "";
                    //PriEngine.Engine.IniciaTransaccao();
                    PriEngine.Engine.Comercial.Vendas.TransformaDocumentoEX(l.ToArray(), myFac, true, strAvisos);
                    //PriEngine.Engine.TerminaTransaccao();
                    if (strAvisos.Length > 0)
                    {
                        erro.Erro = 1;
                        erro.Descricao +=" Transforma Avsos: "+ strAvisos;
                    }
                    else
                    {
                        erro.Erro = 0;
                        erro.Descricao = "Sucesso";
                    }
                    return erro;
                }
                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir empresa";
                    return erro;
                }
            }
            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao +=" Excecao: " +ex.Message;
                return erro;
            }
        }

        public static GcpBEDocumentoVenda DocvendaToBEDocVenda(Model.DocVenda doc)
        {
            GcpBEDocumentoVenda ret = new GcpBEDocumentoVenda();

            ret.set_ID(doc.id);
            ret.set_Entidade(doc.Entidade);
            ret.set_NumDoc(doc.NumDoc);
            ret.set_DataDoc(doc.Data);
            ret.set_TotalMerc(doc.TotalMerc);
            ret.set_Serie(doc.Serie);
            GcpBELinhasDocumentoVenda linhasDoc = new GcpBELinhasDocumentoVenda();

            foreach(Lib_Primavera.Model.LinhaDocVenda linha in doc.LinhasDoc){
                GcpBELinhaDocumentoVenda l = new GcpBELinhaDocumentoVenda();
                l.set_AutoID(linha.IdCabecDoc);
                l.set_Artigo(linha.CodArtigo);
                l.set_Descricao(linha.DescArtigo);
                l.set_Quantidade(linha.Quantidade);
                l.set_Unidade(linha.Unidade);
                l.set_Desconto1(Convert.ToSingle(linha.Desconto));
                l.set_PrecUnit(linha.PrecoUnitario);
                l.set_TotalIliquido(linha.TotalILiquido);
                l.set_TotalDC(linha.TotalLiquido);

                linhasDoc.Insere(l);
            }
            ret.set_Linhas(linhasDoc);


            return ret;

        }


        public static List<Model.DocVenda> Faturas_List()
        {

            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocVenda dv = new Model.DocVenda();
            List<Model.DocVenda> listdv = new List<Model.DocVenda>();
            Model.LinhaDocVenda lindv = new Model.LinhaDocVenda();
            List<Model.LinhaDocVenda> listlindv = new
            List<Model.LinhaDocVenda>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                objListCab = PriEngine.Engine.Consulta("SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='FA'");
                while (!objListCab.NoFim())
                {
                    dv = new Model.DocVenda();
                    dv.id = objListCab.Valor("id");
                    dv.Entidade = objListCab.Valor("Entidade");
                    dv.NumDoc = objListCab.Valor("NumDoc");
                    dv.Data = objListCab.Valor("Data");
                    dv.TotalMerc = objListCab.Valor("TotalMerc");
                    dv.Serie = objListCab.Valor("Serie");
                    objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, DataEntrega from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                    listlindv = new List<Model.LinhaDocVenda>();

                    while (!objListLin.NoFim())
                    {
                        lindv = new Model.LinhaDocVenda();
                        lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                        lindv.CodArtigo = objListLin.Valor("Artigo");
                        lindv.DescArtigo = objListLin.Valor("Descricao");
                        lindv.Quantidade = objListLin.Valor("Quantidade");
                        lindv.Unidade = objListLin.Valor("Unidade");
                        lindv.Desconto = objListLin.Valor("Desconto1");
                        lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");
                        lindv.DataEntrega = objListLin.Valor("DataEntrega");

                        listlindv.Add(lindv);
                        objListLin.Seguinte();
                    }

                    dv.LinhasDoc = listlindv;
                    listdv.Add(dv);
                    objListCab.Seguinte();
                }
            }
            return listdv;
        }

        public static List<Model.DocVenda> Encomendas_List()
        {
            
            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocVenda dv = new Model.DocVenda();
            List<Model.DocVenda> listdv = new List<Model.DocVenda>();
            Model.LinhaDocVenda lindv = new Model.LinhaDocVenda();
            List<Model.LinhaDocVenda> listlindv = new
            List<Model.LinhaDocVenda>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                objListCab = PriEngine.Engine.Consulta("SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL'");
                while (!objListCab.NoFim())
                {
                    dv = new Model.DocVenda();
                    dv.id = objListCab.Valor("id");
                    dv.Entidade = objListCab.Valor("Entidade");
                    dv.NumDoc = objListCab.Valor("NumDoc");
                    dv.Data = objListCab.Valor("Data");
                    dv.TotalMerc = objListCab.Valor("TotalMerc");
                    dv.Serie = objListCab.Valor("Serie");
                    objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, DataEntrega from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                    listlindv = new List<Model.LinhaDocVenda>();

                    while (!objListLin.NoFim())
                    {
                        lindv = new Model.LinhaDocVenda();
                        lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                        lindv.CodArtigo = objListLin.Valor("Artigo");
                        lindv.DescArtigo = objListLin.Valor("Descricao");
                        lindv.Quantidade = objListLin.Valor("Quantidade");
                        lindv.Unidade = objListLin.Valor("Unidade");
                        lindv.Desconto = objListLin.Valor("Desconto1");
                        lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");
                        lindv.DataEntrega = objListLin.Valor("DataEntrega");

                        listlindv.Add(lindv);
                        objListLin.Seguinte();
                    }

                    dv.LinhasDoc = listlindv;
                    listdv.Add(dv);
                    objListCab.Seguinte();
                }
            }
            return listdv;
        }


       

        public static Model.DocVenda Encomenda_Get(string numdoc)
        {
            
            
            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocVenda dv = new Model.DocVenda();
            Model.LinhaDocVenda lindv = new Model.LinhaDocVenda();
            List<Model.LinhaDocVenda> listlindv = new List<Model.LinhaDocVenda>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                

                string st = "SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL' and NumDoc='" + numdoc + "'";
                objListCab = PriEngine.Engine.Consulta(st);
                dv = new Model.DocVenda();
                dv.id = objListCab.Valor("id");
                dv.Entidade = objListCab.Valor("Entidade");
                dv.NumDoc = objListCab.Valor("NumDoc");
                dv.Data = objListCab.Valor("Data");
                dv.TotalMerc = objListCab.Valor("TotalMerc");
                dv.Serie = objListCab.Valor("Serie");
                objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, DataEntrega from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                listlindv = new List<Model.LinhaDocVenda>();

                while (!objListLin.NoFim())
                {
                    lindv = new Model.LinhaDocVenda();
                    lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                    lindv.CodArtigo = objListLin.Valor("Artigo");
                    lindv.DescArtigo = objListLin.Valor("Descricao");
                    lindv.Quantidade = objListLin.Valor("Quantidade");
                    lindv.Unidade = objListLin.Valor("Unidade");
                    lindv.Desconto = objListLin.Valor("Desconto1");
                    lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                    lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                    lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");
                    lindv.DataEntrega = objListLin.Valor("DataEntrega");
                    listlindv.Add(lindv);
                    objListLin.Seguinte();
                }

                dv.LinhasDoc = listlindv;
                return dv;
            }
            return null;
        }

        #endregion DocsVenda

        #region Armazens

        public static List<Model.Armazem> Warehouses_List()
        {

            StdBELista objListCab;
            Model.Armazem dc = new Model.Armazem();
            List<Model.Armazem> listdc = new List<Model.Armazem>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                objListCab = PriEngine.Engine.Consulta("SELECT Armazem, Morada, Localidade, Cp FROM Armazens ORDER BY 'Armazem'");
                while (!objListCab.NoFim())
                {
                    dc = new Model.Armazem();
                    dc.Nome = objListCab.Valor("Armazem");
                    dc.Morada = objListCab.Valor("Morada");
                    dc.Localidade = objListCab.Valor("Localidade");
                    dc.CodPostal = objListCab.Valor("Cp");

                    listdc.Add(dc);
                    objListCab.Seguinte();
                }
            }
            return listdc;
        }

       

        public static List<Model.ArmazemLoc> WarehousesLocation_Get(string nome)
        {

            StdBELista objListCab;
            Model.ArmazemLoc dc = new Model.ArmazemLoc();
            List<Lib_Primavera.Model.ArmazemLoc> lista = new List<Model.ArmazemLoc>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                objListCab = PriEngine.Engine.Consulta("select Artigo, Lote, StkActual, Localizacao from ArtigoArmazem Where Armazem = '" + nome + "' Order by Artigo");
                while (!objListCab.NoFim())
                {
                    dc = new Model.ArmazemLoc();
                    dc.Artigo = objListCab.Valor("Artigo");
                    dc.Lote = objListCab.Valor("Lote");
                    double stock = objListCab.Valor("StkActual");
                    dc.Stock = stock.ToString();
                    dc.Localizacao = objListCab.Valor("Localizacao");

                    lista.Add(dc);
                    objListCab.Seguinte();

                }

                return lista;
            }
            return null;
        }

        public static List<Model.ArmazemLoc> InventoryStock_Get(string id)
        {

            StdBELista objListCab;
            Model.ArmazemLoc dc = new Model.ArmazemLoc();
            List<Lib_Primavera.Model.ArmazemLoc> lista = new List<Model.ArmazemLoc>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                objListCab = PriEngine.Engine.Consulta("select Artigo, Lote, StkActual, Localizacao from ArtigoArmazem Where Artigo =" + " '" + id + "'");
                while (!objListCab.NoFim())
                {
                    dc = new Model.ArmazemLoc();
                    dc.Artigo = objListCab.Valor("Artigo");
                    dc.Lote = objListCab.Valor("Lote");
                    double stock = objListCab.Valor("StkActual");
                    dc.Stock = stock.ToString();
                    dc.Localizacao = objListCab.Valor("Localizacao");

                    lista.Add(dc);
                    objListCab.Seguinte();

                }
                if (!lista.Any())
                {
                    return null;
                }

                return lista;
            }
            return null;
        }

        #endregion Armazens
    }
}