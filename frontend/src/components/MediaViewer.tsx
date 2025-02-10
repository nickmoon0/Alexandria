import { getFile } from "@/features/entries/api/get-file";
import React, { useEffect, useState } from "react";

interface MediaViewerProps {
  documentId: string;
}

enum MediaType {
  image,
  video,
  document
}

const MediaViewer: React.FC<MediaViewerProps> = ({ documentId }) => {
  const [mediaSrc, setMediaSrc] = useState<string | null>(null);
  const [mediaType, setMediaType] = useState<MediaType | null>(null);

  useEffect(() => {
    if (!documentId) return;

    /*const fetchMedia = async () => {
      try {
        const response = await getFile({ documentId });

        setMediaSrc(response.mediaUrl);
        // Use direct API URL for video streaming
        if (response.contentType.startsWith("video/")) {
          setMediaType(MediaType.video);
        } else if (response.contentType.startsWith("image/")) {
          setMediaType(MediaType.image);
        } else {
          console.error("Unsupported content type:", response.contentType);
        }
      } catch (error) {
        console.error("Error fetching media:", error);
      }
    };

    fetchMedia();*/
    setMediaType(MediaType.video);
    setMediaSrc('test');
  }, [documentId]);

  return (
    <div className="pb-4 px-4">
      {mediaType === MediaType.image && mediaSrc && (
        <img className="rounded-md" src={mediaSrc} alt="Document" style={{ maxWidth: "100%" }} />
      )}
      {mediaType === MediaType.video && mediaSrc && (
        <video className="rounded-md" controls style={{ maxWidth: "100%" }}>
          <source src={`http://localhost:5062/api/document/${documentId}`} type="video/mp4" />
          Your browser does not support the video tag.
        </video>
      )}
      {mediaType === null && <p>Loading...</p>}
    </div>
  );
};

export default MediaViewer;
