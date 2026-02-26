import { useState } from 'react';
import { VictoryMenu } from './components/VictoryMenu';
import { DefeatMenu } from './components/DefeatMenu';
import { NetworkErrorMenu } from './components/NetworkErrorMenu';

type MenuType = 'victory' | 'defeat' | 'network' | null;

export default function App() {
  const [activeMenu, setActiveMenu] = useState<MenuType>(null);
  const [coins, setCoins] = useState(150);

  const handlePlayAgain = () => {
    setActiveMenu(null);
    // Логика перезапуска игры
    console.log('Игра перезапущена');
  };

  const handleOffline = () => {
    setActiveMenu(null);
    // Логика офлайн режима
    console.log('Переход в офлайн режим');
  };

  const handleRetry = () => {
    setActiveMenu(null);
    // Логика повторной попытки подключения
    console.log('Повторная попытка подключения');
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-purple-600 via-pink-500 to-orange-400 flex items-center justify-center p-4">
      <div className="text-center">
        <h1 className="text-white text-5xl font-bold mb-8 drop-shadow-lg">
          Демо игровых меню
        </h1>
        
        <div className="bg-white/10 backdrop-blur-md rounded-3xl p-8 space-y-4 max-w-md mx-auto">
          <p className="text-white/90 mb-6">
            Нажмите на кнопку, чтобы посмотреть меню:
          </p>
          
          <button
            onClick={() => setActiveMenu('victory')}
            className="w-full bg-gradient-to-r from-yellow-400 to-orange-500 hover:from-yellow-500 hover:to-orange-600 text-white font-bold py-4 px-8 rounded-2xl shadow-lg hover:shadow-xl transition-all text-xl"
          >
            🏆 Показать меню победы
          </button>

          <button
            onClick={() => setActiveMenu('defeat')}
            className="w-full bg-gradient-to-r from-red-500 to-purple-600 hover:from-red-600 hover:to-purple-700 text-white font-bold py-4 px-8 rounded-2xl shadow-lg hover:shadow-xl transition-all text-xl"
          >
            💀 Показать меню поражения
          </button>

          <button
            onClick={() => setActiveMenu('network')}
            className="w-full bg-gradient-to-r from-gray-600 to-gray-800 hover:from-gray-700 hover:to-gray-900 text-white font-bold py-4 px-8 rounded-2xl shadow-lg hover:shadow-xl transition-all text-xl"
          >
            📡 Показать ошибку сети
          </button>

          <div className="mt-6 pt-6 border-t border-white/20">
            <label className="text-white/90 block mb-2">
              Заработанные монеты:
            </label>
            <input
              type="number"
              value={coins}
              onChange={(e) => setCoins(Number(e.target.value))}
              className="w-full bg-white/20 backdrop-blur-sm border-2 border-white/30 rounded-xl px-4 py-2 text-white text-center text-xl font-bold focus:outline-none focus:border-white/50"
            />
          </div>
        </div>
      </div>

      {/* Отображение активного меню */}
      {activeMenu === 'victory' && (
        <VictoryMenu coins={coins} onPlayAgain={handlePlayAgain} />
      )}
      
      {activeMenu === 'defeat' && (
        <DefeatMenu coins={coins} onPlayAgain={handlePlayAgain} />
      )}
      
      {activeMenu === 'network' && (
        <NetworkErrorMenu onOffline={handleOffline} onRetry={handleRetry} />
      )}
    </div>
  );
}
