﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using FroggerStarter.Enums;
using FroggerStarter.Factory;

namespace FroggerStarter.Model.Animation
{
    /// <summary>
    ///     Class holds the implementation for an Animation
    /// </summary>
    public class Animation : IEnumerable<Frame>
    {
        #region Data Members

        private DispatcherTimer animateTimer;
        private IList<Frame> animationFrames;
        private readonly AnimationType animationType;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the animation interval.
        /// </summary>
        /// <value>
        ///     The animation interval.
        /// </value>
        public int AnimationInterval { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is animation finished.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is animation finished; otherwise, <c>false</c>.
        /// </value>
        private bool IsAnimationFinished => this.animationFrames.All(frame => frame.HasBeenPlayed && !frame.IsVisible);

        /// <summary>
        ///     Gets or sets a value indicating whether [animation has started].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [animation has started]; otherwise, <c>false</c>.
        /// </value>
        public bool AnimationHasStarted { get; private set; }

        #endregion

        #region Events

        /// <summary>
        ///     The animation finished Event Args
        /// </summary>
        public EventHandler<AnimationIsFinishedEventArgs> AnimationFinished;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Animation"/> class.
        ///     Precondition: none
        ///     Post-condition: this.animationFrames.Count()++
        /// </summary>
        /// <param name="animationType">Type of the animation</param>
        public Animation(AnimationType animationType)
        {
            this.animationType = animationType;
            this.createFramesFromAnimationSprites();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Sets the frame locations for all the frames.
        ///     Precondition: none
        ///     Post-condition: @each frame.X = [x]
        ///                           frame.Y = [y]
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void SetFrameLocations(double x, double y)
        {
            foreach (var frame in this.animationFrames)
            {
                frame.X = x;
                frame.Y = y;
            }
        }

        /// <summary>
        ///     Rotates the frames the specified [direction].
        ///     Precondition: none
        ///     Post-condition: @sprite.RenderTransform = direction
        /// </summary>
        /// <param name="direction">The direction.</param>
        public void RotateFrames(Direction direction)
        {
            this.animationFrames.ToList().ForEach(frame => frame.Rotate(direction));
        }

        /// <summary>
        ///     Starts the timer and makes the first frame visible.
        ///     Precondition: none
        ///     Post-condition: this.animationFrames[0].IsVisible = true
        /// </summary>
        public void Start()
        {
            this.animateTimer = new DispatcherTimer();
            this.animateTimer.Tick += this.showNextFrame;
            this.animateTimer.Interval = new TimeSpan(0, 0, 0, 0, this.AnimationInterval);
            
            this.makeFirstFrameVisible();
            this.AnimationHasStarted = true;
            this.animateTimer.Start();
        }

        /// <summary>
        ///     Stops the animate timer.
        /// </summary>
        public void Stop()
        {
            this.animateTimer.Stop();
            this.AnimationHasStarted = false;
        }

        /// <summary>
        ///     Starts the animation on endless loop
        ///     Precondition: none
        ///     Post-condition: this.animationFrames[0].IsVisible = true
        /// </summary>
        public void StartEndlessLoopKeepBaseFrame()
        {
            this.animateTimer = new DispatcherTimer();
            this.animateTimer.Tick += this.showNextFrameEndlessLoopKeepBaseFrame;
            this.animateTimer.Interval = new TimeSpan(0, 0, 0, 0, this.AnimationInterval);

            this.makeFirstFrameVisible();
            this.animateTimer.Start();
            this.AnimationHasStarted = true;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Frame> GetEnumerator()
        {
            return this.animationFrames.GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.animationFrames.GetEnumerator();
        }

        #endregion

        #region Private Helpers

        private void makeFirstFrameVisible()
        {
            this.animationFrames[0].IsVisible = true;
        }

        private void createFramesFromAnimationSprites()
        {
            this.animationFrames = new List<Frame>();
            var listOfSprites = AnimationFactory.CreateAnimationSprites(this.animationType);

            foreach (var sprite in listOfSprites)
            {
                this.animationFrames.Add(new Frame(sprite));
            }
        }

        private void showNextFrame(object sender, object e)
        {
            var firstVisibleFrame = this.animationFrames.First(frame => frame.IsVisible);
            firstVisibleFrame.IsVisible = false;

            var firstInvisibleFrame = this.animationFrames.FirstOrDefault(frame => !frame.IsVisible && !frame.HasBeenPlayed);
            if (firstInvisibleFrame != null)
            {
                firstInvisibleFrame.IsVisible = true;
            }

            this.checkIfAnimationIsFinished();
        }

        private void showNextFrameEndlessLoopKeepBaseFrame(object sender, object e)
        {
            var firstVisibleFrame = this.animationFrames.FirstOrDefault(frame => frame.IsVisible);
            var firstInvisibleFrame = this.animationFrames.FirstOrDefault(frame => !frame.IsVisible && !frame.HasBeenPlayed);
            var numberOfVisibleFrames = this.animationFrames.Count(frame => frame.IsVisible);

            if (firstVisibleFrame != null && numberOfVisibleFrames != 1)
            {
                firstVisibleFrame.IsVisible = false;
            }
            else if (firstInvisibleFrame != null)
            {
                firstInvisibleFrame.IsVisible = true;
            }
            else
            {
                this.animationFrames.ToList().ForEach(frame => frame.ResetStatusAndVisibility());
            }
        }

        private void checkIfAnimationIsFinished()
        {
            if (this.IsAnimationFinished)
            {
                this.animationFrames.ToList().ForEach(frame => frame.ResetStatusAndVisibility());
                this.animateTimer.Stop();
                this.AnimationHasStarted = false;
                this.invokeFinishedAnimationBasedOnType();
            }
        }

        private void invokeFinishedAnimationBasedOnType()
        {
            var animationDone = new AnimationIsFinishedEventArgs();

            switch (this.animationType)
            {
                case AnimationType.PlayerDeath:
                    animationDone.PlayerDeathIsOver = true;
                    break;

                case AnimationType.FrogLeap:
                    animationDone.FrogLeapIsOver = true;
                    break;

                case AnimationType.LifeHeartLost:
                    animationDone.LifeHeartLostIsOver = true;
                    break;

                case AnimationType.QuickReviveHeartLost:
                    animationDone.QuickReviveHeartLostIsOver = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.AnimationFinished?.Invoke(this, animationDone);
        }

        #endregion
    }

    /// <summary>
    ///     Holds the event for finished animation.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class AnimationIsFinishedEventArgs : EventArgs
    {

        /// <summary>
        ///     Gets or sets a value indicating whether [animation is over].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [animation is over]; otherwise, <c>false</c>.
        /// </value>
        public bool PlayerDeathIsOver { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [animation is over].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [animation is over]; otherwise, <c>false</c>.
        /// </value>
        public bool FrogLeapIsOver { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [life heart lost is over].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [life heart lost is over]; otherwise, <c>false</c>.
        /// </value>
        public bool LifeHeartLostIsOver { get; set; }


        /// <summary>
        ///     Gets or sets a value indicating whether [quick revive heart lost is over].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [quick revive heart lost is over]; otherwise, <c>false</c>.
        /// </value>
        public bool QuickReviveHeartLostIsOver { get; set; }
    }
}
