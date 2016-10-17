using System;

namespace LocalBuddies.Mobile.UI.Audio
{
    public interface IAudioHandler
    {
        void StopRecording();
        void StartRecordingAsync(Action<byte[]> callback);
        void StartRecording(Action<byte[]> callback);
        void PlayAudio(byte[] audioBuffer);
    }
}
