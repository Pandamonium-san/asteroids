using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Asteroid
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Song bgm, gameOver_sfx;

        SpriteBatch spriteBatch;
        SpriteFont font1;

        bg bg1, bg2, bg3;
        Texture2D bg1_texture, bg2_texture, bg3_texture, explosion_texture, gameOver_texture, player_explode;

        public Vector2 centerOfWindow, mousePosition;

        List<Asteroid> asteroids;
        Texture2D asteroid_texture;
        Rectangle rHitbox;
        int asteroidSounds;

        Vector2 rPos, rSpeed;
        float rScale;
        int rMinSpeed, rMaxSpeed;

        List<Bullet> bullets;
        Vector2 bulletStartPos, bulletDirection;
        Rectangle bulletHitbox;
        float bulletSpeed;
        Texture2D bullet_texture;
       
        Player player;
        Vector2 playerStartPos, playerAcc;
        Texture2D player_texture;

        Powerup powerup;
        Texture2D powerup_texture;
        float nextPowerup, powerupTime, powerupCounter;
        bool powerupExists, powerupActive, piercingBullets;

        Random rand;

        bool paused, gameOver, gameOverSound, showPlayerInfo;

        float nextShot, shotRate;

        int nextSpawn, SpawnRate;

        int maxAsteroids, diffLevel;

        int playerScore, playerLives;

        float playerTime, playerinfoTime;
        int playerTime1;

        float playerInvulnerableCount, playerInvulnerableTime;

        public string playerinfo;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;


            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            gameOver = false;
            paused = false;
            gameOverSound = false;
            centerOfWindow = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            
            playerStartPos = centerOfWindow;
            playerAcc = new Vector2(0.2f, 0.2f);

            bulletSpeed = 15;
            nextShot = 60;
            shotRate = 7;       //Shots fired per second

            nextSpawn = 0;
            SpawnRate = 300;       //Lower value means higher spawnrate (min time for spawn = spawnrate/5 frames, max time = spawnrate frames)
            rMinSpeed = 1;        //Asteroid min/max speed
            rMaxSpeed = 4;
            maxAsteroids = 10;

            diffLevel = 0;

            playerScore = 0;
            playerLives = 3;
            playerTime = 0;
            playerInvulnerableCount = 0;
            playerInvulnerableTime = 2;     //Invulnerability duration after respawn

            nextPowerup = 1;     //Time before next powerup
            powerupTime = 0;
            powerupExists = false;
            powerupActive = false;
            piercingBullets = false;

            showPlayerInfo = true;
            playerinfo = "WASD to move. Click to shoot. P to pause.";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
           
            bgm = Content.Load<Song>("irisu_05");
            gameOver_sfx = Content.Load<Song>("irisu_gos1");

            bg1_texture = Content.Load<Texture2D>("bg1");
            bg2_texture = Content.Load<Texture2D>("bg2");
            bg3_texture = Content.Load<Texture2D>("bg3");

            asteroid_texture = Content.Load<Texture2D>("asteroid");
            player_texture = Content.Load<Texture2D>("Ship");
            bullet_texture = Content.Load<Texture2D>("Bullet");
            gameOver_texture = Content.Load<Texture2D>("GameOver1");
            explosion_texture = Content.Load<Texture2D>("explosion");
            player_explode = Content.Load<Texture2D>("player_explosion");
            powerup_texture = Content.Load<Texture2D>("powerups");
            font1 = Content.Load<SpriteFont>("font1");

            player = new Player(player_texture, playerStartPos, Vector2.Zero, playerAcc);
            powerup = new Powerup(powerup_texture, Vector2.Zero, Vector2.Zero, new Rectangle(0,0,0,0), 0);

            bg1 = new bg(bg1_texture, new Rectangle(0, 0, bg1_texture.Width, bg1_texture.Height), new Rectangle(0, -bg1_texture.Height, bg1_texture.Width, bg1_texture.Height), 1f, 2);
            bg2 = new bg(bg2_texture, new Rectangle(0, 0, bg2_texture.Width, bg2_texture.Height), new Rectangle(0, -bg2_texture.Height, bg2_texture.Width, bg2_texture.Height), 0.3f, 3);
            bg3 = new bg(bg3_texture, new Rectangle(0, 0, bg3_texture.Width, bg3_texture.Height), new Rectangle(0, -bg3_texture.Height, bg3_texture.Width, bg3_texture.Height), 0.5f, 4);

            asteroids = new List<Asteroid>();
            bullets = new List<Bullet>();
            rand = new Random();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.Play(bgm);
        }

        public void SpawnRandom()   //Method to randomize a position and relevant vectors
        {
            switch (rand.Next(0, 4))
            {
                case 0: //Spawning from right
                    rPos = new Vector2(Window.ClientBounds.Width + 100, rand.Next(0, Window.ClientBounds.Height));
                    rSpeed = new Vector2(rand.Next(-rMaxSpeed, -rMinSpeed), rand.Next(-rMaxSpeed, rMaxSpeed));
                    break;
                case 1: //Spawning from left
                    rPos = new Vector2(-100, rand.Next(0, Window.ClientBounds.Height));
                    rSpeed = new Vector2(rand.Next(rMinSpeed, rMaxSpeed), rand.Next(-rMaxSpeed, rMaxSpeed));
                    break;
                case 2: //Spawning from top
                    rPos = new Vector2(rand.Next(0, Window.ClientBounds.Width), -100);
                    rSpeed = new Vector2(rand.Next(-rMaxSpeed, rMaxSpeed), rand.Next(rMinSpeed, rMaxSpeed));
                    break;
                case 3: //Spawning from bottom
                    rPos = new Vector2(rand.Next(0, Window.ClientBounds.Width), Window.ClientBounds.Height + 100);
                    rSpeed = new Vector2(rand.Next(-rMaxSpeed, rMaxSpeed), rand.Next(-rMaxSpeed, -rMinSpeed));
                    break;
            }
        }

        public void ShootBullet()   //Shoots a bullet at mouse cursor from players position
        {
            bulletStartPos = player.getPos();

            //Aim at mouse logic
            mousePosition = KeyMouseReader.getNewPos();             
            bulletDirection = mousePosition - bulletStartPos;
            if(bulletDirection != Vector2.Zero)
                bulletDirection.Normalize();

            bulletHitbox = new Rectangle((int)bulletStartPos.X, (int)bulletStartPos.Y, bullet_texture.Width, bullet_texture.Height);
            bullets.Add(new Bullet(bullet_texture, bulletStartPos, bulletDirection, bulletSpeed, bulletHitbox));
            soundBank.PlayCue("se_damage00");
        }

        public void DisplayPlayerInfo(string info)
        {
            playerinfo = info;
            showPlayerInfo = true;
            playerinfoTime = 0;
        }

        protected override void Update(GameTime gameTime)
        {
            KeyMouseReader.Update();

            if (!gameOverSound && gameOver)
            {
                MediaPlayer.Stop();
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Play(gameOver_sfx);
                gameOverSound = true;
            }
            if (gameOver)
            {
                playerinfo = "Press enter to restart";
                showPlayerInfo = true;
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    Initialize();
            }


            if (KeyMouseReader.KeyPressed(Keys.P))
            {
                paused = !paused;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (showPlayerInfo)
                playerinfoTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (playerinfoTime > 9)
            {
                showPlayerInfo = false;
                playerinfoTime = 0;
            }

            audioEngine.Update();

            if (gameOver || paused) return;

                this.Window.Title = "AsteroidKiller       Score: " + playerScore.ToString();
                player.Update();
                bg1.Update();
                bg2.Update();
                bg3.Update();

                //Difficulty
                diffLevel = playerTime1 / 15 + playerScore/10000;
                maxAsteroids = 10 + diffLevel * 3;
                rMinSpeed = 1 + diffLevel / 8;
                rMaxSpeed = 4 + diffLevel / 4;
                if (SpawnRate > 1)
                    SpawnRate = 200 - diffLevel * 7;
                if (SpawnRate < 1)
                    SpawnRate = 1;
               
                //Bullet logic
                foreach (Bullet bullet in bullets)
                {
                    bullet.Update();

                    foreach (Asteroid asteroid in asteroids)
                    {
                        asteroid.Collision(bullet.hitbox);
                        if(!piercingBullets)                       
                        bullet.Collision(asteroid.hitbox);
                    }

                }

                if (KeyMouseReader.LeftClickPressed() && nextShot >= 60 && !player.dead)
                {
                    ShootBullet();
                    nextShot = 0;
                }
                if (nextShot < 60)
                {
                    nextShot += shotRate;
                }

                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    if (bullets[i].bulletHitAsteroid)
                        playerScore += 100;
                    if (bullets[i].removeBullet)
                        bullets.RemoveAt(i);
                }


                //Asteroid Logic
                foreach (Asteroid asteroid in asteroids)
                {
                    asteroid.Update();
                    player.Collision(asteroid.hitbox);
                }

                foreach (Asteroid asteroid in asteroids)
                {
                    foreach (Asteroid asteroid2 in asteroids)
                    {
                        if (asteroid2 != asteroid && asteroid.scale > 0.7 && asteroid2.scale > 0.7)     //Collision with other asteroids
                        {
                            asteroid.Collision(asteroid2.hitbox);
                            asteroid2.Collision(asteroid.hitbox);
                        }
                    }
                    if (asteroid.destroyAsteroid)
                    {
                        asteroid.texture = explosion_texture;
                    }
                }
                foreach (Asteroid asteroid in asteroids)
                {
                    if (asteroid.spawnDaughters)        //Logic for spawning "daughter" asteroids
                    {
                        rHitbox = new Rectangle((int)asteroid.pos.X, (int)asteroid.pos.Y, asteroid_texture.Width, asteroid_texture.Height);
                        Vector2 daughterSpeed, daughterSpeed2;
                        daughterSpeed = new Vector2(asteroid.speed.X, rand.Next(-2,2));
                        daughterSpeed2 = new Vector2(rand.Next(-2,2), asteroid.speed.Y);
                        asteroids.Add(new Asteroid(asteroid_texture, asteroid.pos, daughterSpeed, rHitbox, asteroid.scale / 2));
                        asteroids.Add(new Asteroid(asteroid_texture, asteroid.pos, daughterSpeed2, rHitbox, asteroid.scale / 2));
                        asteroid.spawnDaughters = false;
                        break;
                    }

                }

                nextSpawn += rand.Next(1, 5);
                if (nextSpawn >= SpawnRate && asteroids.Count < maxAsteroids)
                {
                    SpawnRandom();                                  //Randomize position/speed of asteroid
                    rScale = (11 - rand.Next(1, 4)) * 0.12f;        //Randomize size of asteroid
                    rHitbox = new Rectangle((int)rPos.X, (int)rPos.Y, asteroid_texture.Width, asteroid_texture.Height);
                    asteroids.Add(new Asteroid(asteroid_texture, rPos, rSpeed, rHitbox, rScale));
                    nextSpawn = 0;
                }

                for (int i = asteroids.Count - 1; i >= 0; i--)      //Goes through list in reverse order to avoid rearrangement in list
                {
                    if (asteroids[i].destroyAsteroid && asteroids[i].animationTime > 10)
                    {
                        if (asteroidSounds < 3)           //Prevents sound overlap from more than 3 explosions on the same frame
                        {
                            soundBank.PlayCue("boom6");
                            ++asteroidSounds;
                        }
                        asteroids.RemoveAt(i);
                    }
                }
                asteroidSounds = 0; 


                //Player Logic
                if (player.dead)
                {
                    player.texture = player_explode;
                    if (player.animationTime == 1)
                        soundBank.PlayCue("boom1");
                    if (player.animationTime >= 100)    //Checks to see whether animation has ended
                    {
                        player.pos = playerStartPos;
                        player.speed = Vector2.Zero;
                        player.texture = player_texture;
                        playerLives -= 1;
                        player.invulnerable = true;
                        player.dead = false;
                        
                    }
                    if (player.animationTime >= 100)
                        player.animationTime = 0;
                }


                if (playerLives <= 0)
                    gameOver = true;

                if (player.invulnerable)
                    playerInvulnerableCount += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (playerInvulnerableCount >= playerInvulnerableTime)
                {
                    player.invulnerable = false;
                    playerInvulnerableCount = 0;
                }
                playerTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                playerTime1 = (int)playerTime;

                //Powerup logic
                powerup.Update();
                if (!powerupExists)
                    powerupCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (powerupCounter >= nextPowerup)
                {
                    SpawnRandom();
                    rHitbox = new Rectangle((int)rPos.X, (int)rPos.Y, powerup_texture.Width, powerup_texture.Height);
                    powerup = new Powerup(powerup_texture, rPos, rSpeed*0.5f, rHitbox, rand.Next(1, 6));
                    powerupExists = true;
                    nextPowerup = rand.Next(10,20);
                    powerupCounter = 0;
                }
                powerup.Collision(player.hitbox);
                if (powerup.playerGotPowerup)
                {
                    
                    switch (powerup.id)
                    {
                        case 1:
                            playerLives += 1;
                            DisplayPlayerInfo("                  Life up!");
                            soundBank.PlayCue("se_bonus");
                            break;
                        case 2:
                            shotRate = 14;
                            powerupTime = 0;
                            powerupActive = true;
                            DisplayPlayerInfo("            Fire rate doubled!");
                            soundBank.PlayCue("se_ch02");
                            break;
                        case 3:
                            foreach (Asteroid asteroid in asteroids)
                                asteroid.destroyAsteroid = true;
                            DisplayPlayerInfo("                KABOOM");
                            soundBank.PlayCue("se_slash");
                            break;
                        case 4:
                            player.invulnerable = true;
                            playerInvulnerableCount = -7;
                            DisplayPlayerInfo("           Invulnerability!");
                            soundBank.PlayCue("se_chargeup");
                            break;
                        case 5:
                            piercingBullets = true;
                            shotRate = 5;
                            powerupTime = 0;
                            powerupActive = true;
                            DisplayPlayerInfo("         Asteroids are made of cheese!");
                            soundBank.PlayCue("se_charge00");
                            break;
                    }
                    powerup = new Powerup(powerup_texture, Vector2.Zero, Vector2.Zero, new Rectangle(-10, -10, 0, 0), 0);
                    powerupExists = false;
                }
                if (powerupActive)
                    powerupTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (powerupTime >= 10)
                {
                    shotRate = 7;
                    piercingBullets = false;
                    powerupActive = false;
                    powerupTime = 0;
                }

            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            bg1.Render(spriteBatch);
            bg2.Render(spriteBatch);
            bg3.Render(spriteBatch);

                foreach (Asteroid asteroid in asteroids)
                {
                    asteroid.Render(spriteBatch);
                }
                foreach (Bullet bullet in bullets)
                {
                    bullet.Render(spriteBatch);
                }
                
            if(powerupExists)
                    powerup.Render(spriteBatch);

                player.Render(spriteBatch);

                spriteBatch.DrawString(font1, "Asteroids " + asteroids.Count.ToString(), new Vector2(5, Window.ClientBounds.Height - 28), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font1, "Score " + playerScore.ToString(), new Vector2(5, 5), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font1, "Time " + playerTime1.ToString(), new Vector2(Window.ClientBounds.Width - 100, 0), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font1, "Lives " + playerLives.ToString(), new Vector2(Window.ClientBounds.Width - 80, Window.ClientBounds.Height - 28), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                
            if (showPlayerInfo)
                spriteBatch.DrawString(font1, playerinfo, new Vector2(450, 0), Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);

            if (gameOver)
            {
                spriteBatch.Draw(gameOver_texture, centerOfWindow, null, Color.Red, 0f, new Vector2(gameOver_texture.Width / 2, gameOver_texture.Height / 2), 1f, SpriteEffects.None, 0f);
            }
            if (paused)
                spriteBatch.DrawString(font1, "GAME PAUSED", new Vector2(Window.ClientBounds.Width/2-110, Window.ClientBounds.Height/2-30), Color.White, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0.5f);
              
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
