﻿using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using FroggerStarter.Constants;
using FroggerStarter.Controller;
using FroggerStarter.Model.Player;
using FroggerStarter.Model.Score;
using Color = Windows.UI.Color;
using Size = Windows.Foundation.Size;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FroggerStarter.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage
    {
        #region Data members

        private readonly double applicationHeight = (double) Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double) Application.Current.Resources["AppWidth"];
 
        private readonly GameManager gameManager;
        private readonly DispatcherTimer endGameTimer;

        #endregion

        #region Constants

        private const int EndGameTimeInterval = 5;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePage"/> class.
        /// </summary>
        public GamePage()
        {
            this.InitializeComponent();

            ApplicationView.GetForCurrentView().TryResizeView(
                new Size(Width = this.applicationWidth, Height = this.applicationHeight)
            );
            
            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;

            this.initializeGameBoardConstants();

            this.gameManager = new GameManager();
            this.gameManager.ScoreUpdated += this.onScoreUpdated;
            this.gameManager.GameOver += this.onGameOver;
            this.gameManager.ScoreTimerTick += this.onScoreTimerTick;
            this.gameManager.NextLevel += this.onNextLevel;
            this.gameManager.PauseFinished += this.onPauseFinished;

            this.gameManager.InitializeGame(this.canvas);
            this.endGameTimer = new DispatcherTimer();
        }

        private void initializeGameBoardConstants()
        {
            GameBoard.BackgroundWidth     = this.applicationWidth;
            GameBoard.BackgroundHeight    = this.applicationHeight;
            GameBoard.HighRoadYLocation   =    (double) Application.Current.Resources["HighRoadYLocation"];
            GameBoard.BottomRoadYLocation =    (double) Application.Current.Resources["BottomRoadYLocation"];
            GameBoard.MiddleRoadYLocation =    (double) Application.Current.Resources["MiddleRoadYLocation"];
            GameBoard.HomeWidth           =    (double) Application.Current.Resources["HomeWidth"];
            GameBoard.HomeLocationGapSize =    (double) Application.Current.Resources["HomeLocationGapSize"];
            GameBoard.RoadShoulderOffset  =    (double) Application.Current.Resources["RoadShoulderOffset"];
            GameBoard.StatHudBottomYLocation = (double) Application.Current.Resources["StatHudBottomYLocation"];
        }

        #endregion

        #region Methods

        private void coreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    this.gameManager.MovePlayerLeft();
                    break;
                case VirtualKey.Right:
                    this.gameManager.MovePlayerRight();
                    break;
                case VirtualKey.Up:
                    this.gameManager.MovePlayerUp();
                    break;
                case VirtualKey.Down:
                    this.gameManager.MovePlayerDown();
                    break;
            }
        }

        private void onScoreTimerTick(object sender, ScoreTimerTickEventArgs e)
        {
            if (e.ScoreTick > GameSettings.ScoreTime)
            {
                this.timeProgressBar.Foreground = new SolidColorBrush(Color.FromArgb(255, 218, 165, 32));
                this.timeTwoValue.Visibility = Visibility.Visible;
            }
            else
            {
                this.timeProgressBar.Foreground = new SolidColorBrush(Color.FromArgb(255, 50, 205, 50));
                this.timeTwoValue.Visibility = Visibility.Collapsed;
            }

            if (this.timeProgressBar.Value >= this.timeProgressBar.Minimum)
            {
                this.timeProgressBar.Value = e.ScoreTick;
            }
        }

        private void onNextLevel(object sender, NextLevelEventArgs e)
        {
            this.nextLevel.Text = e.NextLevel.ToString();
            this.blackOverlay.Visibility = Visibility.Visible;
            this.level.Visibility = Visibility.Visible;
            this.nextLevel.Visibility = Visibility.Visible;
        }

        private void onPauseFinished(object sender, PauseIsFinishedEventArgs e)
        {
            this.blackOverlay.Visibility = Visibility.Collapsed;
            this.nextLevel.Visibility = Visibility.Collapsed;
            this.level.Visibility = Visibility.Collapsed;
        }
        
        private void onScoreUpdated(object sender, ScoreUpdatedEventArgs e)
        {
            this.scoreValue.Text = e.Score.ToString();
        }

        private void onGameOver(object sender, GameOverEventArgs e)
        {
            if (e.GameOver)
            {
                this.gameOver.Visibility = Visibility.Visible;
                this.blackOverlay.Visibility = Visibility.Visible;
            }

            this.endGameTimer.Tick += this.endGameTimerOnTick;            
            this.endGameTimer.Interval = new TimeSpan(0, 0, 0, EndGameTimeInterval);
            this.endGameTimer.Start();
        }

        private void endGameTimerOnTick(object sender, object e)
        {
            if (this.endGameTimer.IsEnabled)
            {
                this.endGameTimer.Stop();

                this.gameManager.ResetGame();
                Window.Current.CoreWindow.KeyDown -= this.coreWindowOnKeyDown;

                this.Frame.Navigate(typeof(AddHighScorePage));
            }
        }

        #endregion
    }
}