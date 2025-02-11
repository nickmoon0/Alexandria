import { getFile } from "@/features/entries/api/get-file";
import { getDocumentParams } from "@/lib/document-service";
import React, { useEffect, useState } from "react";
import Button from "@/components/button";

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

  const retrieveDocumentParams = async () => {
    const documentParams = await getDocumentParams({ documentId });
    if (documentParams !== null) {
      setMediaSrc(documentParams.url);

      // Determine media type based on content type
      const contentType = documentParams.contentType.toLowerCase();
      if (contentType.startsWith("image/")) {
        setMediaType(MediaType.image);
      } else if (contentType.startsWith("video/")) {
        setMediaType(MediaType.video);
      } else {
        setMediaType(MediaType.document); // Default to document type if not image/video
      }
    }
  };

  useEffect(() => {
    retrieveDocumentParams();
  }, [documentId]);

  const handleDownload = () => {
    if (mediaSrc) {
      const link = document.createElement("a");
      link.href = mediaSrc;
      link.download = `document-${documentId}`; // Default filename
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  };

  return (
    <div className="pb-4 px-4">
      {mediaType === MediaType.image && mediaSrc && (
        <img className="rounded-md" src={mediaSrc} alt="Document" style={{ maxWidth: "100%" }} />
      )}
      {mediaType === MediaType.video && mediaSrc && (
        <video className="rounded-md" controls style={{ maxWidth: "100%" }}>
          <source src={mediaSrc} type="video/mp4" />
          Your browser does not support the video tag.
        </video>
      )}
      {mediaType === MediaType.document && mediaSrc && (
        <Button onClick={handleDownload}>
          Download
        </Button>
      )}
      {mediaType === null && <p>Loading...</p>}
    </div>
  );
};

export default MediaViewer;
