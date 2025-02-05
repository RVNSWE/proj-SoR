using Microsoft.Xna.Framework;
using SoR.Logic.GameMap;
using System;
using System.Collections.Generic;

namespace SoR.Logic.Character
{
    /*
     * This class handles player input and animation application.
     */
    public partial class Entity
    {
        protected Random random;
        protected Vector2 newPosition;
        protected Vector2 direction;
        protected bool idle;
        protected float sinceLastChange;
        protected float newDirectionTime;
        protected float sinceFreeze;
        public bool Frozen { get; set; }
        public bool Traversable { get; set; }
        public bool DirectionReversed { get; set; }
        public int CountDistance { get; set; }
        public bool BeenPushed { get; set; }

        /*
         * Calculate the direction to be repelled in. Positive to move right or down, negative to move up or left.
         */
        public float RepelDirection(float direction, bool positive)
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
         * Be repelled away from an entity.
         */
        public void RepelledFromEntity(int distance, Entity entity)
        {
            CountDistance = distance;

            direction = Vector2.Zero;

            if (entity.Position.X > Position.X) // Push right
            {
                direction.X = RepelDirection(direction.X, false);
            }
            else if (entity.Position.X < Position.X) // Push left
            {
                direction.X = RepelDirection(direction.X, true);
            }
            if (entity.Position.Y > Position.Y) // Push down
            {
                direction.Y = RepelDirection(direction.Y, false);
            }
            else if (entity.Position.Y < Position.Y) // Push up
            {
                direction.Y = RepelDirection(direction.Y, true);
            }
        }

        /*
         * Be repelled away from scenery.
         */
        public void RepelledFromScenery(int distance, Scenery scenery)
        {
            CountDistance = distance;

            direction = Vector2.Zero;

            if (scenery.GetPosition().X > Position.X) // Push right
            {
                direction.X = RepelDirection(direction.X, false);
            }
            else if (scenery.GetPosition().X < Position.X) // Push left
            {
                direction.X = RepelDirection(direction.X, true);
            }
            if (scenery.GetPosition().Y > Position.Y) // Push down
            {
                direction.Y = RepelDirection(direction.Y, false);
            }
            else if (scenery.GetPosition().Y < Position.Y) // Push up
            {
                direction.Y = RepelDirection(direction.Y, true);
            }
        }

        /*
         * Change direction to move away from something.
         */
        public void RedirectNPC()
        {
            if (newPosition.X > prevPosition.X)
            {
                NewDirection(1); // Redirect left
            }
            else if (newPosition.X < prevPosition.X)
            {
                NewDirection(2); // Redirect right
            }

            if (newPosition.Y > prevPosition.Y)
            {
                NewDirection(3); // Redirect up
            }
            else if (newPosition.Y < prevPosition.Y)
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
                    RedirectAnimation(1);
                    break;
                case 2:
                    direction = new Vector2(1, 0); // Right
                    RedirectAnimation(2);
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
         * Animate NPC redirection.
         */
        public void RedirectAnimation(int newDirection)
        {
            switch (newDirection)
            {
                case 1:
                    ChangeAnimation("turnleft");
                    GetSkeleton().ScaleX = 1;
                    break;
                case 2:
                    ChangeAnimation("turnright");
                    GetSkeleton().ScaleX = -1;
                    break;
            }
        }

        /*
         * Freeze entity movement for a short while after spawning.
         */
        public void FrozenTimer(GameTime gameTime)
        {
            float deltaTime = GameLogic.GetTime(gameTime);
            float freezeTime = 1f;
            sinceFreeze += deltaTime;

            if (sinceFreeze >= freezeTime)
            {
                Frozen = false;
            }
        }

        /*
         * Move the NPC in the direction they're facing, and periodically pick a random new direction.
         */
        public void NonPlayerMovement(GameTime gameTime)
        {
            float deltaTime = GameLogic.GetTime(gameTime);
            int newDirection;
            sinceLastChange += deltaTime;
            newPosition = Position;

            if (IsMoving())
            {
                if (sinceLastChange >= newDirectionTime || BeenPushed)
                {
                    newDirection = random.Next(4);
                    NewDirection(newDirection);
                    newDirectionTime = (float)random.NextDouble() * 3f + 0.33f;
                    sinceLastChange = 0;
                    BeenPushed = false;
                }
            }

            prevPosition = Position;
        }

        /*
         * Set the new position after moving, and halve the speed if moving diagonally.
         */
        public void AdjustPosition(GameTime gameTime, List<Rectangle> impassableArea)
        {
            newPosition = Position;

            float newSpeed = (float)(Speed * 1.5) * GameLogic.GetTime(gameTime);

            if (direction.X > 0 | direction.X < 0 && direction.Y > 0 | direction.Y < 0) // If moving diagonally
            {
                newSpeed /= 1.5f; // Reduce the speed by 25%
            }

            newPosition += direction * newSpeed;

            foreach (Rectangle area in impassableArea)
            {
                if (area.Contains(newPosition) && !area.Contains(prevPosition))
                {
                    direction = Vector2.Zero;

                    if (!Player) // If entity is not the player
                    {
                        if (!DirectionReversed) // If the direction has not already been reversed
                        {
                            RedirectNPC(); // Move in the opposite direction
                            DirectionReversed = true;
                        }
                    }

                    Traversable = false;
                    newPosition = prevPosition;

                    break;
                }
                if (area.Contains(newPosition) && area.Contains(prevPosition)) // If entity is stuck inside the wall
                {
                    bool left = prevPosition.X < area.Center.X;
                    bool right = prevPosition.X > area.Center.X;
                    bool top = prevPosition.Y < area.Center.Y;
                    bool bottom = prevPosition.Y > area.Center.Y;

                    if (left) // If it is in the left half of the wall
                    {
                        newPosition.X -= newSpeed; // Move the entity left
                    }
                    else if (right)
                    {
                        newPosition.X += newSpeed;
                    }

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

            Position = newPosition;
            prevPosition = Position;
        }
    }
}