using System;
using TMPro;
using UnityEngine;

namespace Runtime.Game
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text turnsText;
        [SerializeField] private TMP_Text comboText;
        
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private GameObject finalScoreObject;
        
        public int Score { get; private set; }
        public int Combo { get; private set; }
        public int Turns { get; private set; }
        
        private void Update()
        {
            // Using Observable pattern would be an improvement, but to keep it vanilla, we will directly call it
            RefreshView();
        }

        public void Reset()
        {
            ResetCombo();
            ResetScore();
            HideFinalScore();
        }
        
        public void SuccessfulMatch()
        {
            AddTurn();
            AddScore(1 * (Combo + 1));
            AddCombo();
        }
        
        public void FailedMatch()
        {
            AddTurn();
            ResetCombo();
        }
        
        public void ShowFinalScore()
        {
            finalScoreObject.SetActive(true);
            finalScoreText.text = $"Final Score: {Score}";
        }
        
        private void HideFinalScore()
        {
            finalScoreObject.SetActive(false);
        }
        
        private void RefreshView()
        {
            scoreText.text = $"Score: {Score}";
            comboText.text = $"Combo: {Combo}";
            turnsText.text = $"Turns: {Turns}";
        }

        private void AddTurn()
        {
            Turns += 1;
        }

        private void AddScore(int score)
        {
            Score += score;
        }

        private void ResetScore()
        {
            Score = 0;
        }
        
        private void AddCombo()
        {
            Combo += 1;
        }
        
        private void ResetCombo()
        {
            Combo = 0;
        }
    }
}
