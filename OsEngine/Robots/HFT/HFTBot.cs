using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.Market;
using OsEngine.Market.Servers;
using OsEngine.Market.Servers.Kraken.KrakenEntity;
using OsEngine.OsTrader.Panels;

namespace OsEngine.Robots.HFT
{
    public class HFTBot : BotPanel
    {
        public HFTBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            ServerMaster.ServerCreateEvent += ServerMaster_ServerCreateEvent;//событие на подключение сервера
        }

        #region Fields =========================================================================

        private List<IServer> _servers = new List<IServer>();//список серверов

        private List<Portfolio> _portfolios = new List<Portfolio>();//список счетов

        private string _nameSecurity = "SiH3+SPBFUT";

        private ServerType _serverType = ServerType.QuikLua;

        private Security _security = null;

        private IServer _server;

        private CandleSeries _series = null;

        #endregion



        #region Methods ========================================================================

        private void ServerMaster_ServerCreateEvent(IServer newServer)
        {
            foreach (IServer server in _servers)//пробегаемся по списку серверов, если он уже есть в списке - выходим 
            {
                if (newServer == server)
                {
                    return;
                }
            }

            if (newServer.ServerType == ServerType.QuikLua)
            {
                _server = newServer;
            }

            _servers.Add(newServer); //если нет добавляем в список

            newServer.PortfoliosChangeEvent += NewServer_PortfoliosChangeEvent;//событие на обновление счета
            newServer.SecuritiesChangeEvent += NewServer_SecuritiesChangeEvent;//событие на обновление бумаг с биржи
            newServer.NeadToReconnectEvent += NewServer_NeadToReconnectEvent;//событие на перезаказ данных с сервера
            newServer.NewMarketDepthEvent += NewServer_NewMarketDepthEvent;//подписка на обновление стакана
            newServer.NewTradeEvent += NewServer_NewTradeEvent;//подписка на обезличенные сделки
            newServer.NewOrderIncomeEvent += NewServer_NewOrderIncomeEvent;//изменение ордера
            newServer.NewMyTradeEvent += NewServer_NewMyTradeEvent;//произошла моя сделка

        }

        private void NewServer_NewMyTradeEvent(MyTrade myTrade)
        {
            
        }

        private void NewServer_NewOrderIncomeEvent(Order order)
        {
            
        }

        private void NewServer_NewTradeEvent(List<Trade> trades)
        {
            
        }

        private void NewServer_NewMarketDepthEvent(MarketDepth marketDepth)
        {
            
        }

        private void NewServer_NeadToReconnectEvent()
        {
            StartSecurity(_security);
        }

        private void NewServer_SecuritiesChangeEvent(List<Security> securities)
        {
            if (_security != null)//если переменная уже содержит бумагу - выходим из цикла
            {
                return;
            }

            for (int i = 0; i < securities.Count; i++)//добавляем в поле _security нужную бумагу из списка
            {
                if (_nameSecurity == securities[i].Name)
                {
                    _security = securities[i];

                    StartSecurity(_security);
                    break;
                }
            }
        }

        private void StartSecurity(Security security)
        {
            if (security == null)
            {
                Debug.WriteLine("StartSecurity security = null");
                return;
            }

            Task.Run(() =>
            {
                while (true)
                {
                    _series = _server.StartThisSecurity(security.Name, new TimeFrameBuilder(), security.NameClass);

                    if (_series != null)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            });
            

        }

        private void NewServer_PortfoliosChangeEvent(List<Portfolio> newportfolios)
        {
            for (int x = 0; x < newportfolios.Count; x++)//добавляем в список счетов новые портфели
            {

                bool flag = true;

                for (int i = 0; i < _portfolios.Count; i++)
                {
                    if (newportfolios[x].Number == _portfolios[i].Number)
                    {
                        flag = false;
                        break;
                    }
                    
                }
                if (flag)
                {
                    _portfolios.Add(newportfolios[x]);
                }
            }

            
        }

        public override string GetNameStrategyType()
        {
            return nameof(HFTBot);
        }

        public override void ShowIndividualSettingsDialog()
        {

        }

        #endregion

    }
}
