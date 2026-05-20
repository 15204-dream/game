export type Role = 'guest' | 'director';

export type Gender = 'male' | 'female';

export interface Guest {
  id: number;
  nickname: string;
  name: string;
  gender: Gender;
  avatar: string;
  age: number;
  occupation: string;
  appearance: string;
  personality: string;
  background: string;
  loveType: string;
}

export interface GameState {
  role: Role | null;
  playerGender: Gender | null;
  playerAvatar: string | null;
  day: number;
  maxDays: number;
  guests: Guest[];
  affection: Record<number, number>;
  friendship: Record<number, number>;
  gold: number;
  diamond: number;
  inventory: string[];
  tasks: Task[];
  messages: string[];
  choices: ChoiceRecord[];
  // 导演视角专有
  heat: number;
  ratings: number[];
}

export interface Task {
  id: string;
  title: string;
  description: string;
  completed: boolean;
  type: 'daily' | 'main' | 'achievement';
  reward: { gold?: number; diamond?: number; item?: string };
}

export interface ChoiceRecord {
  day: number;
  choiceId: string;
  choiceText: string;
  timestamp: number;
}

export interface Message {
  id: string;
  from: string;
  to: string;
  content: string;
  day: number;
  time: string;
  read: boolean;
}
