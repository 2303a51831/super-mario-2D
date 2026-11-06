using UnityEngine;

namespace SuperUnity
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Sources")]
        public AudioSource sfxSource;
        public AudioSource musicSource;

        [Header("Audio Clips (Optional - procedural synth used if null)")]
        public AudioClip jumpSound;
        public AudioClip coinSound;
        public AudioClip powerUpSound;
        public AudioClip powerDownSound;
        public AudioClip stompSound;
        public AudioClip kickSound;
        public AudioClip brickBreakSound;
        public AudioClip bumpSound;
        public AudioClip fireballSound;
        public AudioClip flagpoleSound;
        public AudioClip stageClearSound;
        public AudioClip gameOverSound;
        public AudioClip dieSound;
        public AudioClip oneUpSound;

        private void Awake()
        {
            if (Instance != null && Instance != FindAnyObjectByType<SoundManager>())
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
            }
            musicSource.volume = SuperUnity.Managers.SaveManager.MusicVolume;
            if (sfxSource != null) sfxSource.volume = SuperUnity.Managers.SaveManager.SFXVolume;

            GenerateProceduralAudioClips();
            PlayMainTheme();
        }

        public void SetMusicVolume(float vol)
        {
            if (musicSource != null) musicSource.volume = Mathf.Clamp01(vol);
            SuperUnity.Managers.SaveManager.MusicVolume = vol;
        }

        public void SetSFXVolume(float vol)
        {
            if (sfxSource != null) sfxSource.volume = Mathf.Clamp01(vol);
            SuperUnity.Managers.SaveManager.SFXVolume = vol;
        }

        private void GenerateProceduralAudioClips()
        {
            if (jumpSound == null) jumpSound = CreateJumpClip();
            if (coinSound == null) coinSound = CreateCoinClip();
            if (powerUpSound == null) powerUpSound = CreatePowerUpClip();
            if (powerDownSound == null) powerDownSound = CreatePowerDownClip();
            if (stompSound == null) stompSound = CreateStompClip();
            if (kickSound == null) kickSound = CreateKickClip();
            if (brickBreakSound == null) brickBreakSound = CreateBrickBreakClip();
            if (bumpSound == null) bumpSound = CreateBumpClip();
            if (fireballSound == null) fireballSound = CreateFireballClip();
            if (flagpoleSound == null) flagpoleSound = CreateFlagpoleClip();
            if (stageClearSound == null) stageClearSound = CreateStageClearClip();
            if (gameOverSound == null) gameOverSound = CreateGameOverClip();
            if (dieSound == null) dieSound = CreateDieClip();
            if (oneUpSound == null) oneUpSound = CreateOneUpClip();
        }

        public void PlayJump() => PlaySfx(jumpSound);
        public void PlayCoin() => PlaySfx(coinSound);
        public void PlayPowerUp() => PlaySfx(powerUpSound);
        public void PlayPowerDown() => PlaySfx(powerDownSound);
        public void PlayStomp() => PlaySfx(stompSound);
        public void PlayKick() => PlaySfx(kickSound);
        public void PlayBrickBreak() => PlaySfx(brickBreakSound);
        public void PlayBump() => PlaySfx(bumpSound);
        public void PlayFireball() => PlaySfx(fireballSound);
        public void PlayFlagpole() => PlaySfx(flagpoleSound);
        public void PlayStageClear()
        {
            StopMusic();
            PlaySfx(stageClearSound);
        }
        public void PlayGameOver()
        {
            StopMusic();
            PlaySfx(gameOverSound);
        }
        public void PlayDie()
        {
            StopMusic();
            PlaySfx(dieSound);
        }
        public void PlayOneUp() => PlaySfx(oneUpSound);

        private void PlaySfx(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, 0.8f);
            }
        }

        public void PlayMainTheme()
        {
            if (musicSource != null)
            {
                AudioClip theme = CreateOverworldThemeClip();
                musicSource.clip = theme;
                musicSource.Play();
            }
        }

        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        // Procedural Audio Synthesizers (8-bit NES style)
        private AudioClip CreateJumpClip()
        {
            int sampleRate = 44100;
            float duration = 0.28f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float freq = Mathf.Lerp(150f, 600f, Mathf.Pow(t, 0.6f));
                phase += freq / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.4f : -0.4f;
                float env = 1f - t;
                samples[i] = square * env;
            }

            AudioClip clip = AudioClip.Create("JumpSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateCoinClip()
        {
            int sampleRate = 44100;
            float duration = 0.35f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float freq = t < 0.35f ? 987.77f : 1318.51f; // B5 -> E6
                phase += freq / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.35f : -0.35f;
                float env = 1f - Mathf.Pow(t, 0.7f);
                samples[i] = square * env;
            }

            AudioClip clip = AudioClip.Create("CoinSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateStompClip()
        {
            int sampleRate = 44100;
            float duration = 0.15f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float freq = Mathf.Lerp(280f, 60f, t);
                phase += freq / sampleRate;
                if (phase > 1f) phase -= 1f;

                float wave = phase < 0.5f ? 0.5f : -0.5f;
                wave += (Random.value * 2f - 1f) * 0.2f * (1f - t);
                float env = 1f - t;
                samples[i] = wave * env;
            }

            AudioClip clip = AudioClip.Create("StompSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateKickClip()
        {
            int sampleRate = 44100;
            float duration = 0.18f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float freq = Mathf.Lerp(450f, 90f, t);
                phase += freq / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.5f : -0.5f;
                samples[i] = square * (1f - t);
            }

            AudioClip clip = AudioClip.Create("KickSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreatePowerUpClip()
        {
            int sampleRate = 44100;
            float duration = 0.7f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float[] notes = { 330f, 392f, 659f, 523f, 587f, 784f };
            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                int noteIndex = Mathf.FloorToInt(t * notes.Length);
                noteIndex = Mathf.Clamp(noteIndex, 0, notes.Length - 1);
                float freq = notes[noteIndex];

                phase += freq / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.35f : -0.35f;
                samples[i] = square * (1f - (t * 0.4f));
            }

            AudioClip clip = AudioClip.Create("PowerUpSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreatePowerDownClip()
        {
            int sampleRate = 44100;
            float duration = 0.55f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float freq = Mathf.Lerp(600f, 120f, t);
                phase += freq / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.4f : -0.4f;
                samples[i] = square * (1f - t);
            }

            AudioClip clip = AudioClip.Create("PowerDownSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateBrickBreakClip()
        {
            int sampleRate = 44100;
            float duration = 0.3f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float noise = (Random.value * 2f - 1f) * 0.45f;
                samples[i] = noise * (1f - t * t);
            }

            AudioClip clip = AudioClip.Create("BrickBreakSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateBumpClip()
        {
            int sampleRate = 44100;
            float duration = 0.12f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float freq = Mathf.Lerp(120f, 50f, t);
                phase += freq / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.45f : -0.45f;
                samples[i] = square * (1f - t);
            }

            AudioClip clip = AudioClip.Create("BumpSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateFireballClip()
        {
            int sampleRate = 44100;
            float duration = 0.15f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float freq = Mathf.Lerp(800f, 200f, t);
                phase += freq / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.35f : -0.35f;
                samples[i] = square * (1f - t);
            }

            AudioClip clip = AudioClip.Create("FireballSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateFlagpoleClip()
        {
            int sampleRate = 44100;
            float duration = 1.2f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float freq = Mathf.Lerp(500f, 100f, t);
                phase += freq / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.35f : -0.35f;
                samples[i] = square * (1f - (t * 0.3f));
            }

            AudioClip clip = AudioClip.Create("FlagpoleSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateDieClip()
        {
            int sampleRate = 44100;
            float duration = 1.8f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            // Classic Mario death melody: B4 F5 F5 F5 E5 D5 C5
            float[] notes = { 493.88f, 698.46f, 698.46f, 698.46f, 659.25f, 587.33f, 523.25f };
            float[] noteLengths = { 0.2f, 0.15f, 0.15f, 0.25f, 0.25f, 0.25f, 0.45f };

            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float time = (float)i / sampleRate;
                float accumulated = 0f;
                int activeNote = 0;
                for (int n = 0; n < noteLengths.Length; n++)
                {
                    accumulated += noteLengths[n];
                    if (time < accumulated)
                    {
                        activeNote = n;
                        break;
                    }
                }

                float freq = notes[activeNote];
                phase += freq / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.35f : -0.35f;
                samples[i] = square * (1f - (time / duration));
            }

            AudioClip clip = AudioClip.Create("DieSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateStageClearClip()
        {
            int sampleRate = 44100;
            float duration = 3.5f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float[] fanfareNotes = { 392f, 523f, 659f, 784f, 1046f, 784f, 1046f };
            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                int nIdx = Mathf.Clamp(Mathf.FloorToInt(t * fanfareNotes.Length), 0, fanfareNotes.Length - 1);
                phase += fanfareNotes[nIdx] / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.35f : -0.35f;
                samples[i] = square * (1f - (t * 0.5f));
            }

            AudioClip clip = AudioClip.Create("StageClearSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateGameOverClip()
        {
            int sampleRate = 44100;
            float duration = 2.5f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float[] notes = { 523.25f, 392.00f, 329.63f, 440.00f, 493.88f, 440.00f, 415.30f, 466.16f, 415.30f, 392.00f };
            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                int nIdx = Mathf.Clamp(Mathf.FloorToInt(t * notes.Length), 0, notes.Length - 1);
                phase += notes[nIdx] / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.35f : -0.35f;
                samples[i] = square * (1f - (t * 0.7f));
            }

            AudioClip clip = AudioClip.Create("GameOverSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateOneUpClip()
        {
            int sampleRate = 44100;
            float duration = 0.45f;
            int sampleCount = Mathf.FloorToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float[] notes = { 330f, 392f, 659f, 523f, 587f, 784f };
            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                int nIdx = Mathf.Clamp(Mathf.FloorToInt(t * notes.Length), 0, notes.Length - 1);
                phase += notes[nIdx] / sampleRate;
                if (phase > 1f) phase -= 1f;

                float square = phase < 0.5f ? 0.35f : -0.35f;
                samples[i] = square * (1f - t);
            }

            AudioClip clip = AudioClip.Create("OneUpSFX", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateOverworldThemeClip()
        {
            int sampleRate = 22050; // Optimized sample rate for theme loop
            // Classic Mario Overworld Intro + Loop
            float[] melody = {
                659.25f, 659.25f, 0f, 659.25f, 0f, 523.25f, 659.25f, 0f, 783.99f, 0f, 0f, 0f, 392.00f, 0f, 0f, 0f,
                523.25f, 0f, 0f, 392.00f, 0f, 0f, 329.63f, 0f, 0f, 440.00f, 0f, 493.88f, 0f, 466.16f, 440.00f, 0f,
                392.00f, 659.25f, 783.99f, 880.00f, 0f, 698.46f, 783.99f, 0f, 659.25f, 0f, 523.25f, 587.33f, 493.88f, 0f
            };
            float noteDur = 0.12f;
            float totalDuration = melody.Length * noteDur;
            int sampleCount = Mathf.FloorToInt(sampleRate * totalDuration);
            float[] samples = new float[sampleCount];

            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float time = (float)i / sampleRate;
                int step = Mathf.FloorToInt(time / noteDur);
                if (step >= melody.Length) step = melody.Length - 1;

                float freq = melody[step];
                if (freq > 10f)
                {
                    phase += freq / sampleRate;
                    if (phase > 1f) phase -= 1f;

                    float stepProgress = (time % noteDur) / noteDur;
                    float square = phase < 0.5f ? 0.25f : -0.25f;
                    float env = 1f - Mathf.Pow(stepProgress, 3f);
                    samples[i] = square * env;
                }
                else
                {
                    samples[i] = 0f;
                }
            }

            AudioClip clip = AudioClip.Create("OverworldTheme", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
