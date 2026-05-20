import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useGame } from '../context/GameContext';
import type { Role } from '../types';

const SelectRolePage: React.FC = () => {
  const navigate = useNavigate();
  const { setRole } = useGame();

  const handleSelectRole = (role: Role) => {
    setRole(role);
    if (role === 'guest') {
      navigate('/guest-game');
    } else {
      navigate('/director-game');
    }
  };

  return (
    <div style={styles.container}>
      <h2 style={styles.title}>选择你的身份</h2>
      <div style={styles.cardsContainer}>
        <div style={styles.card} onClick={() => handleSelectRole('guest')}>
          <div style={styles.cardContent}>
            <div style={styles.cardIcon}>💑</div>
            <h3 style={styles.cardTitle}>嘉宾视角</h3>
            <p style={styles.cardDescription}>
              作为嘉宾入住心动小屋，寻找你的真爱！</p>
          </div>
        </div>
        <div style={styles.card} onClick={() => handleSelectRole('director')}>
          <div style={styles.cardContent}>
            <div style={styles.cardIcon}>🎬</div>
            <h3 style={styles.cardTitle}>导演视角</h3>
            <p style={styles.cardDescription}>
              作为导演，打造爆款恋综！</p>
          </div>
        </div>
      </div>
      <button style={styles.backButton} onClick={() => navigate('/')}>
        返回
      </button>
    </div>
  );
};

const styles = {
  container: {
    display: 'flex' as const,
    flexDirection: 'column' as const,
    alignItems: 'center' as const,
    justifyContent: 'center' as const,
    minHeight: '100vh',
    backgroundColor: '#FFF0F5',
    padding: '20px'
  },
  title: {
    fontSize: '36px',
    color: '#FF6B6B',
    marginBottom: '40px',
    textAlign: 'center' as const
  },
  cardsContainer: {
    display: 'flex' as const,
    gap: '32px',
    marginBottom: '40px',
    flexWrap: 'wrap' as const,
    justifyContent: 'center' as const
  },
  card: {
    width: '300px',
    backgroundColor: 'white',
    borderRadius: '16px',
    padding: '32px',
    cursor: 'pointer' as const,
    boxShadow: '0 4px 12px rgba(0,0,0,0.1)',
    transition: 'transform 0.2s'
  },
  cardContent: {
    textAlign: 'center' as const
  },
  cardIcon: {
    fontSize: '64px',
    marginBottom: '16px'
  },
  cardTitle: {
    fontSize: '24px',
    color: '#333',
    marginBottom: '12px'
  },
  cardDescription: {
    fontSize: '16px',
    color: '#666',
    lineHeight: '1.6'
  },
  backButton: {
    padding: '12px 24px',
    fontSize: '16px',
    backgroundColor: 'transparent',
    color: '#FF6B6B',
    border: '2px solid #FF6B6B',
    borderRadius: '8px',
    cursor: 'pointer'
  }
};

export default SelectRolePage;
