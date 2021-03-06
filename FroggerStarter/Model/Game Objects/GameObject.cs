﻿using System;
using System.Drawing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using FroggerStarter.Enums;
using FroggerStarter.View.Sprites;
using Point = Windows.Foundation.Point;

namespace FroggerStarter.Model.Game_Objects
{
    /// <summary>
    ///     Defines basic properties and behavior of every game object.
    /// </summary>
    public abstract class GameObject
    {
        #region Data members

        private Point location;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the x location of the game object.
        /// </summary>
        /// <value>
        ///     The x.
        /// </value>
        public double X
        {
            get => this.location.X;
            set
            {
                this.location.X = value;
                this.render();
            }
        }

        /// <summary>
        ///     Gets or sets the y location of the game object.
        /// </summary>
        /// <value>
        ///     The y.
        /// </value>
        public double Y
        {
            get => this.location.Y;
            set
            {
                this.location.Y = value;
                this.render();
            }
        }

        /// <summary>
        ///     Gets the width of the game object.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public double Width => this.Sprite.Width;

        /// <summary>
        ///     Gets the height of the game object.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public double Height => this.Sprite.Height;

        /// <summary>
        ///     Gets or sets the sprite associated with the game object.
        /// </summary>
        /// <value>
        ///     The sprite.
        /// </value>
        public BaseSprite Sprite { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Determines whether [has collided with] [the specified other game object].
        ///     Precondition: otherGameObject != null
        ///     Post-condition: none
        /// </summary>
        /// <param name="otherGameObject">The other game object.</param>
        /// <returns>
        ///   <c>true</c> if [has collided with] [the specified other game object]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasCollidedWith(GameObject otherGameObject)
        {
            if (otherGameObject == null)
            {
                throw new NullReferenceException();
            }

            var otherObjectBoundary = new Rectangle(
                (int)otherGameObject.X,
                (int)otherGameObject.Y,
                (int)otherGameObject.Width,
                (int)otherGameObject.Height
            );

            var thisObjectBoundary = new Rectangle(
                (int)this.X ,
                (int)this.Y,
                (int)this.Width,
                (int)this.Height
            );

            return thisObjectBoundary.IntersectsWith(otherObjectBoundary);
        }

        /// <summary>
        ///     Determines whether [has collided with] [the specified other game object].
        ///     But it squeezes the x location on right/left side of collision boundary.
        ///     Needed to make the frog have more a realistic landing on water obstacles etc.
        ///     Precondition: otherGameObject != null
        ///     Post-condition: none
        /// </summary>
        /// <param name="otherGameObject">The other game object.</param>
        /// <returns>
        ///   <c>true</c> if [has collided with] [the specified other game object]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasCollidedMoreThanHalfOfSprite(GameObject otherGameObject)
        {
            if (otherGameObject == null)
            {
                throw new NullReferenceException();
            }

            var otherObjectBoundary = new Rectangle(
                (int)otherGameObject.X + (int) this.Width / 2,
                (int)otherGameObject.Y,
                (int)otherGameObject.Width - (int) this.Width,
                (int)otherGameObject.Height
            );

            var thisObjectBoundary = new Rectangle(
                (int)this.X,
                (int)this.Y,
                (int)this.Width,
                (int)this.Height
            );

            return thisObjectBoundary.IntersectsWith(otherObjectBoundary);
        }

        /// <summary>
        ///     Changes the sprite visibility based on the bool condition passed in.
        ///     Precondition: none
        ///     Post-condition: Sprite.Visibility == (Visible || Collapsed)
        /// </summary>
        /// <param name="conditionToChangeVisibility">if set to <c>true</c> [condition to change visibility].</param>
        public virtual void ChangeSpriteVisibility(bool conditionToChangeVisibility)
        {
            this.Sprite.Visibility = conditionToChangeVisibility ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        ///     Rotates the specified Direction.
        ///     Precondition: none
        ///     Post-condition: @sprite.RenderTransform = Direction
        /// </summary>
        /// <param name="direction">The Direction.</param>
        /// <exception cref="ArgumentOutOfRangeException">Direction - null</exception>
        public void Rotate(Direction direction)
        {
            var rotateTransform = new RotateTransform
            {
                CenterX = this.Width / 2,
                CenterY = this.Height / 2
            };

            switch (direction)
            {
                case Direction.Right:
                    rotateTransform.Angle = 90;
                    break;

                case Direction.Left:
                    rotateTransform.Angle = -90;
                    break;

                case Direction.Up:
                    rotateTransform.Angle = 0;
                    break;

                case Direction.Down:
                    rotateTransform.Angle = 180;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            this.Sprite.RenderTransform = rotateTransform;
        }

        /// <summary>
        ///     Sets the centered y location of an area on the board,
        ///     then returns the centered location with the current yLocation.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="areaHeight">Height of the area.</param>
        /// <param name="yLocation">The y location.</param>
        public void SetCenteredYLocationOfArea(double areaHeight, double yLocation)
        {
            this.Y = ((areaHeight - this.Height) / 2) + yLocation;
        }

        /// <summary>
        ///     Flips the sprite horizontally
        /// </summary>
        public virtual void FlipSpriteHorizontally()
        {
            this.Sprite.RenderTransformOrigin = new Point(0.5, 0.5);
            this.Sprite.RenderTransform = new ScaleTransform() { ScaleX = -1 };
        }

        #endregion

        #region Private Helpers

        private void render()
        {
            this.Sprite.RenderAt(this.X, this.Y);
        }

        #endregion
    }
}