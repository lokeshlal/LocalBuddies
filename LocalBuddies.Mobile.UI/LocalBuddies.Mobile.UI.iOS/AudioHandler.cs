using AudioToolbox;
using AVFoundation;
using Foundation;
using LocalBuddies.Mobile.UI.Audio;
using LocalBuddies.Mobile.UI.iOS;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioHandler))]
namespace LocalBuddies.Mobile.UI.iOS
{
    public class AudioHandler : IAudioHandler
    {
        InputAudioQueue audioQueue;
        //OutputAudioQueue outputQueue;
        private int bufferLength = 1000;

        public bool IsRecording { get; set; }

        public AudioHandler()
        {
            // fileStream = new AudioFileStream(AudioFileType.MP3);
            var session = AVAudioSession.SharedInstance();
            session.SetCategory(AVAudioSessionCategory.PlayAndRecord);
            //session.SetActive(true);
            var output = session.CurrentRoute.Outputs.First();
            NSError err;
            session.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out err);
        }

        public void PlayAudio(byte[] audioBuffer)
        {
            using (NSData data = NSData.FromArray(audioBuffer))
            {
                AVAudioPlayer player = AVAudioPlayer.FromData(data);
                player.FinishedPlaying += delegate
                {
                    player = null;
                };
                player.Play();
            }
        }

        public void StartRecording(Action<byte[]> callback)
        {
            if (IsRecording) return; // already running dont start again
            IsRecording = true;

            var audioFormat = new AudioStreamBasicDescription()
            {
                SampleRate = 11025,
                Format = AudioFormatType.LinearPCM,
                FormatFlags = AudioFormatFlags.LinearPCMIsSignedInteger | AudioFormatFlags.LinearPCMIsPacked,
                FramesPerPacket = 1,
                ChannelsPerFrame = 1,
                BitsPerChannel = 2,
                BytesPerPacket = 2,
                BytesPerFrame = 2,
                Reserved = 0
            };

            audioQueue = new InputAudioQueue(audioFormat);
            audioQueue.InputCompleted += (sender, e) =>
            {
                var buffer = (AudioQueueBuffer)System.Runtime.InteropServices.Marshal.PtrToStructure(e.IntPtrBuffer, typeof(AudioQueueBuffer));
                var send = new byte[buffer.AudioDataByteSize];
                System.Runtime.InteropServices.Marshal.Copy(buffer.AudioData, send, 0, (int)buffer.AudioDataByteSize);
                callback(send);
            };

            var bufferByteSize = bufferLength * audioFormat.BytesPerPacket;

            IntPtr bufferPtr;
            for (var index = 0; index < 3; index++)
            {
                audioQueue.AllocateBufferWithPacketDescriptors(bufferByteSize, this.bufferLength, out bufferPtr);
                audioQueue.EnqueueBuffer(bufferPtr, bufferByteSize, null);
            }

            audioQueue.Start();
        }

        public void StartRecordingAsync(Action<byte[]> callback)
        {
            Task.Run(() =>
            {
                //audioTrack.Play();
                StartRecording(callback);

                var session = AVAudioSession.SharedInstance();
                session.SetActive(true);
            }); //.ConfigureAwait(false);
        }

        public void StopRecording()
        {
            IsRecording = false;
            this.audioQueue.Stop(true);

            var session = AVAudioSession.SharedInstance();
            session.SetActive(false);
            //audioTrack.Stop();
        }
    }
}
