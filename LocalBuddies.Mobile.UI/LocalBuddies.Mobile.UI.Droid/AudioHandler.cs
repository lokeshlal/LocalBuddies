using System;
using LocalBuddies.Mobile.UI.Audio;
using Android.Media;
using System.Threading.Tasks;
using Xamarin.Forms;
using LocalBuddies.Mobile.UI.Droid;

[assembly: Dependency(typeof(AudioHandler))]
namespace LocalBuddies.Mobile.UI.Droid
{
    public class AudioHandler : IAudioHandler
    {
        AudioTrack audioTrack;
        private int bufferLength = 1000;
        /// <summary>
        /// Sets, if mic is recording
        /// </summary>
        public bool IsRecording { get; set; }
        public AudioHandler()
        {
            // starts the audio track and set the stream to voice call
            audioTrack = new AudioTrack(
              // Stream type
              Stream.VoiceCall,
              // Frequency
              11025,
              // Mono or stereo
              ChannelOut.Mono,
              // Audio encoding
              Android.Media.Encoding.Pcm16bit,
              // Length of the audio clip.
              bufferLength,
              // Mode. Stream or static.
              AudioTrackMode.Stream);

            audioTrack.Play();
        }

        public void StopRecording()
        {
            IsRecording = false;
            audioTrack.Stop();
        }

        /// <summary>
        /// starts recording asycn to avoid blocking main UI thread
        /// </summary>
        /// <param name="callback"></param>
        public void StartRecordingAsync(Action<byte[]> callback)
        {
            Task.Run(() =>
            {
                audioTrack.Play();
                StartRecording(callback);
            }); //.ConfigureAwait(false);
        }

        /// <summary>
        /// start the recording and send the recorded bytes to a callback
        /// </summary>
        /// <param name="callback">Callback to handle recorded bytes</param>
        public void StartRecording(Action<byte[]> callback)
        {
            if (IsRecording) return; // already running dont start again
            IsRecording = true;
            byte[] audioBuffer = new byte[bufferLength];
            var audRecorder = new AudioRecord(
               // Hardware source of recording.
               AudioSource.Mic,
               // Frequency
               11025,
               // Mono or stereo
               ChannelIn.Mono,
               // Audio encoding
               Android.Media.Encoding.Pcm16bit,
               // Length of the audio clip.
               audioBuffer.Length
             );

            audRecorder.StartRecording();
            while (IsRecording)
            {
                try
                {
                    // Keep reading the buffer while there is audio input.
                    audRecorder.Read(audioBuffer, 0, audioBuffer.Length);
                    // Write out the audio file.
                    callback?.Invoke(audioBuffer);
                }
                catch (Exception ex)
                {
                    IsRecording = false;
                    System.Diagnostics.Debug.WriteLine("EXCEption : " + ex.Message + " -- " + ex.StackTrace);
                    break;
                }
            }
            audRecorder.Stop();
            audRecorder.Release();
        }

        /// <summary>
        /// play the recieved audio bytes
        /// </summary>
        /// <param name="audioBuffer">audio bytes</param>
        public void PlayAudio(byte[] audioBuffer)
        {
            audioTrack.Write(audioBuffer, 0, audioBuffer.Length);
        }
    }
}