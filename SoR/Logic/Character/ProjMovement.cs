using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SoR.Logic.Character
{
    /*
     * Handles player input and animation application.
     */
    public partial class Projectile
    {
        protected Random random;
        protected Vector2 newPosition;
        protected Vector2 direction;
        protected Vector2 prevDirection;
        protected bool idle;
        protected float sinceLastChange;
        protected float newDirectionTime;
        protected float collisionSeconds;
        protected float frozenSeconds;
        protected float pauseSeconds;
        public bool Frozen { get; set; }
        public bool Cast { get; set; }
        public bool Traversable { get; set; }
        public float CountDistance { get; set; }
        public bool BeenPushed { get; set; }

        /*
         * Calculate the direction to be repelled in. Positive to move right or down, negative to move up or left.
         */
        public static float RepelDirection(float direction, bool positive)
        {
            if (positive)
            {
                direction += 1;
                if (direction > 1)
                {
                    direction = 1;
                }
            }
            else
            {
                direction -= 1;
                if (direction < -1)
                {
                    direction = -1;
                }
            }

            return direction;
        }

        /*
         * Be launched away from a position.
         */
        public void LaunchDistanceFromXY(int distance, float x, float y)
        {
            CountDistance = distance;

            prevDirection = direction;
            direction = Vector2.Zero;

            if (x > position.X) // Push right
            {
                direction.X = RepelDirection(direction.X, false);
            }
            else if (x < position.X) // Push left
            {
                direction.X = RepelDirection(direction.X, true);
            }
            if (y > position.Y) // Push down
            {
                direction.Y = RepelDirection(direction.Y, false);
            }
            else if (y < position.Y) // Push up
            {
                direction.Y = RepelDirection(direction.Y, true);
            }
        }

        /*
         * Change direction to move away from something.
         */
        public void Redirect()
        {
            if (newPosition.X > position.X)
            {
                NewDirection(1); // Redirect left
            }
            else if (newPosition.X < position.X)
            {
                NewDirection(2); // Redirect right
            }

            if (newPosition.Y > position.Y)
            {
                NewDirection(3); // Redirect up
            }
            else if (newPosition.Y < position.Y)
            {
                NewDirection(4); // Redirect down
            }
        }

        /*
         * Choose a new direction to face.
         */
        public void NewDirection(int newDirection)
        {
            switch (newDirection)
            {
                case 1:
                    direction = new Vector2(-1, 0); // Left
                    if (direction != prevDirection)
                    {
                        RedirectAnimation(1);
                    }
                    break;
                case 2:
                    direction = new Vector2(1, 0); // Right
                    if (direction != prevDirection)
                    {
                        RedirectAnimation(2);
                    }
                    break;
                case 3:
                    direction = new Vector2(0, -1); // Up
                    break;
                case 4:
                    direction = new Vector2(0, 1); // Down
                    break;
            }
        }

        /*
         * Animate redirection.
         */
        public virtual void RedirectAnimation(int newDirection)
        {
            switch (newDirection)
            {
                case 1:
                    GetSkeleton().ScaleX = 1;
                    break;
                case 2:
                    GetSkeleton().ScaleX = -1;
                    break;
            }
        }

        /*
         * Freeze entity movement for a short while after spawning.
         */
        public void CheckIfFrozen(GameTime gameTime)
        {
            float deltaTime = GameLogic.GetTime(gameTime);
            frozenSeconds -= deltaTime;

            if (frozenSeconds <= 0)
            {
                Frozen = false;
            }
        }

        /*
         * Move in the direction it's facing, and periodically pick a random new direction.
         */
        /*public void CheckMovement(GameTime gameTime)
        {
            float deltaTime = GameLogic.GetTime(gameTime);
            int newDirection;
            sinceLastChange += deltaTime;
            newPosition = position;

            if (!Pausing)
            {
                if (sinceLastChange >= newDirectionTime || BeenPushed)
                {
                    prevDirection = direction;
                    newDirection = random.Next(4);
                    movementAnimation = "run";
                    NewDirection(newDirection);
                    newDirectionTime = (float)random.NextDouble() * 3f + 0.33f;
                    sinceLastChange = 0;
                    BeenPushed = false;
                }
            }

            position = newPosition;
        }*/

        /*
         * Calculate movement speed. 75% speed if moving diagonally.
         */
        public void CalculateSpeed(GameTime gameTime)
        {
            newSpeed = (float)(Speed * 1.5) * GameLogic.GetTime(gameTime);

            if (direction.X > 0 | direction.X < 0 && direction.Y > 0 | direction.Y < 0)
            {
                newSpeed /= 1.25f;
            }
        }

        /*
         * Set the new Position after moving, and halve the speed if moving diagonally.
         */
        public void AdjustXPosition(List<Rectangle> impassableArea)
        {
            newPosition.X = position.X;

            if (Cast)
            {
                newPosition.X += direction.X * newSpeed;
            }

                foreach (Rectangle area in impassableArea)
                {
                    if (area.Contains(newPosition) && !area.Contains(position))
                    {
                        direction.X = 0;

                    if (Bouncey)
                    {
                        prevDirection = direction;
                        Redirect(); // Move in the opposite direction
                    }
                    else
                    {
                        Colliding = true;
                    }

                        Traversable = false;
                        newPosition.X = position.X;

                        break;
                    }
                    if (area.Contains(newPosition) && area.Contains(position)) // If stuck inside the wall
                    {
                        bool left = position.X < area.Center.X;
                        bool right = position.X > area.Center.X;

                        if (left) // If in the left half of the wall
                        {
                            newPosition.X -= newSpeed; // Move left
                        }
                        else if (right)
                        {
                            newPosition.X += newSpeed;
                        }
                    }
                    else
                    {
                        Traversable = true;
                    }
                }

            position.X = newPosition.X;
        }

        /*
         * Set the new Position after moving, and halve the speed if moving diagonally.
         */
        public void AdjustYPosition(List<Rectangle> impassableArea)
        {
            newPosition.Y = position.Y;

            if (Cast)
            {
                newPosition.Y += direction.Y * newSpeed;
            }

            foreach (Rectangle area in impassableArea)
            {
                if (area.Contains(newPosition) && !area.Contains(position))
                {
                    direction.Y = 0;

                    if (Bouncey)
                    {
                        prevDirection = direction;
                        Redirect(); // Move in the opposite direction
                    }
                    else
                    {
                        Colliding = true;
                    }

                    Traversable = false;
                    newPosition.Y = position.Y;

                    break;
                }
                if (area.Contains(newPosition) && area.Contains(position)) // If entity is stuck inside the wall
                {
                    bool top = position.Y < area.Center.Y;
                    bool bottom = position.Y > area.Center.Y;

                    if (top)
                    {
                        newPosition.Y -= newSpeed;
                    }
                    else if (bottom)
                    {
                        newPosition.Y += newSpeed;
                    }
                }
                else
                {
                    Traversable = true;
                }
            }

            position.Y = newPosition.Y;
        }
    }
}