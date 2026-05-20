import React, { createContext, useContext, useState, type ReactNode } from 'react';
import type { GameState, Role, Gender, Task, ChoiceRecord } from '../types';
import { guests } from '../data/guests';

interface GameContextType {
  gameState: GameState;
  setRole: (role: Role) => void;
  setPlayerGender: (gender: Gender) => void;
  setPlayerAvatar: (avatar: string) => void;
  setDay: (day: number) => void;
  updateAffection: (guestId: number, value: number) => void;
  updateFriendship: (guestId: number, value: number) => void;
  addGold: (amount: number) => void;
  addDiamond: (amount: number) => void;
  addTask: (task: Task) => void;
  completeTask: (taskId: string) => void;
  addChoice: (choice: ChoiceRecord) => void;
  updateHeat: (value: number) => void;
  addRating: (rating: number) => void;
  resetGame: () => void;
  saveGame: () => void;
  loadGame: () => boolean;
}

// 初始化所有嘉宾的好感度和友谊度为0
const initialAffection: Record<number, number> = {};
const initialFriendship: Record<number, number> = {};
guests.forEach(guest => {
  initialAffection[guest.id] = 0;
  initialFriendship[guest.id] = 0;
});

const initialState: GameState = {
  role: null,
  playerGender: null,
  playerAvatar: null,
  day: 1,
  maxDays: 21,
  guests: guests,
  affection: initialAffection,
  friendship: initialFriendship,
  gold: 1000,
  diamond: 100,
  inventory: [],
  tasks: [],
  messages: [],
  choices: [],
  heat: 50,
  ratings: []
};

const GameContext = createContext<GameContextType | undefined>(undefined);

export const GameProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [gameState, setGameState] = useState<GameState>(initialState);

  const setRole = (role: Role) => {
    setGameState(prev => ({ ...prev, role }));
  };

  const setPlayerGender = (gender: Gender) => {
    setGameState(prev => ({ ...prev, playerGender: gender }));
  };

  const setPlayerAvatar = (avatar: string) => {
    setGameState(prev => ({ ...prev, playerAvatar: avatar }));
  };

  const setDay = (day: number) => {
    setGameState(prev => ({ ...prev, day }));
  };

  const updateAffection = (guestId: number, value: number) => {
    setGameState(prev => ({
      ...prev,
      affection: {
        ...prev.affection,
        [guestId]: Math.min(100, Math.max(0, (prev.affection[guestId] || 0) + value))
      }
    }));
  };

  const updateFriendship = (guestId: number, value: number) => {
    setGameState(prev => ({
      ...prev,
      friendship: {
        ...prev.friendship,
        [guestId]: Math.min(100, Math.max(0, (prev.friendship[guestId] || 0) + value))
      }
    }));
  };

  const addGold = (amount: number) => {
    setGameState(prev => ({
      ...prev,
      gold: Math.max(0, prev.gold + amount)
    }));
  };

  const addDiamond = (amount: number) => {
    setGameState(prev => ({
      ...prev,
      diamond: Math.max(0, prev.diamond + amount)
    }));
  };

  const addTask = (task: Task) => {
    setGameState(prev => ({
      ...prev,
      tasks: [...prev.tasks, task]
    }));
  };

  const completeTask = (taskId: string) => {
    setGameState(prev => ({
      ...prev,
      tasks: prev.tasks.map(task => 
        task.id === taskId ? { ...task, completed: true } : task
      )
    }));
  };

  const addChoice = (choice: ChoiceRecord) => {
    setGameState(prev => ({
      ...prev,
      choices: [...prev.choices, choice]
    }));
  };

  const updateHeat = (value: number) => {
    setGameState(prev => ({
      ...prev,
      heat: Math.min(100, Math.max(0, prev.heat + value))
    }));
  };

  const addRating = (rating: number) => {
    setGameState(prev => ({
      ...prev,
      ratings: [...prev.ratings, rating]
    }));
  };

  const resetGame = () => {
    setGameState(initialState);
  };

  const saveGame = () => {
    localStorage.setItem('loveDatingGame', JSON.stringify(gameState));
  };

  const loadGame = (): boolean => {
    const saved = localStorage.getItem('loveDatingGame');
    if (saved) {
      const loaded = JSON.parse(saved);
      // 确保加载的数据与当前的guest数据匹配
      setGameState({
        ...loaded,
        guests: guests
      });
      return true;
    }
    return false;
  };

  return (
    <GameContext.Provider
      value={{
        gameState,
        setRole,
        setPlayerGender,
        setPlayerAvatar,
        setDay,
        updateAffection,
        updateFriendship,
        addGold,
        addDiamond,
        addTask,
        completeTask,
        addChoice,
        updateHeat,
        addRating,
        resetGame,
        saveGame,
        loadGame
      }}
    >
      {children}
    </GameContext.Provider>
  );
};

export const useGame = () => {
  const context = useContext(GameContext);
  if (context === undefined) {
    throw new Error('useGame must be used within a GameProvider');
  }
  return context;
};
