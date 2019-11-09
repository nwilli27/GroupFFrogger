﻿
using System;
using System.Collections.Generic;
using FroggerStarter.Enums;
using FroggerStarter.View.FrogLeapAnimation;
using FroggerStarter.View.LifeHeartLostAnimation;
using FroggerStarter.View.PlayerDeathAnimation;
using FroggerStarter.View.Sprites;

namespace FroggerStarter.Factory
{

    /// <summary>
    ///     Factory class that returns a collection of BaseSprites in order of an animation.
    /// </summary>
    internal class AnimationFactory
    {

        /// <summary>
        ///     Creates the animation sprites list according to the [animationType]
        ///     and returns that list.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="animationType">Type of the animation.</param>
        /// <returns>
        ///     A list of base sprites in order of the animation.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">animationType - null</exception>
        public static List<BaseSprite> CreateAnimationSprites(AnimationType animationType)
        {
            switch (animationType)
            {
                case AnimationType.PlayerDeath:
                    var deathAnimation = new List<BaseSprite>();

                    deathAnimation.Add(new PlayerDeathFrameOne());
                    deathAnimation.Add(new PlayerDeathFrameTwo());
                    deathAnimation.Add(new PlayerDeathFrameThree());
                    deathAnimation.Add(new PlayerDeathFrameFour());

                    return deathAnimation;

                case AnimationType.FrogLeap:
                    var frogLeapAnimation = new List<BaseSprite>();
                    frogLeapAnimation.Add(new FrogLeapFrameOne());
                    return frogLeapAnimation;

                case AnimationType.LifeHeartLost:
                    var heartLostAnimation = new List<BaseSprite>();

                    heartLostAnimation.Add(new LifeHeartLostFrameOne());
                    heartLostAnimation.Add(new LifeHeartLostFrameTwo());
                    heartLostAnimation.Add(new LifeHeartLostFrameThree());
                    heartLostAnimation.Add(new LifeHeartLostFrameFour());
                    heartLostAnimation.Add(new LifeHeartLostFrameFive());
                    heartLostAnimation.Add(new LifeHeartLostFrameSix());
                    heartLostAnimation.Add(new LifeHeartLostFrameSeven());
                    heartLostAnimation.Add(new LifeHeartLostFrameEight());

                    return heartLostAnimation;

                case AnimationType.SpeedBoatSplash:
                    var speedBoatSplashAnimation = new List<BaseSprite>();

                    speedBoatSplashAnimation.Add(new SplashBoatFrameOne());
                    speedBoatSplashAnimation.Add(new SplashBoatFrameTwo());
                    speedBoatSplashAnimation.Add(new SplashBoatFrameThree());

                    return speedBoatSplashAnimation;

                default:
                    throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null);
            }
        }
    }
}
