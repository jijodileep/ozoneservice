# Chunk 18: Job Photos

**Wave:** 4 — Service jobs  
**Depends on:** [chunk-16](chunk-16-flutter-job-detail.md)  
**Status:** pending

## Goal

Upload device condition photos from mobile; view on web.

## Tasks

- [ ] `JobPhoto` entity or blob metadata table
- [ ] File storage: local `wwwroot/uploads` for dev; abstract `IFileStorage`
- [ ] `POST /api/jobs/{id}/photos` (multipart)
- [ ] `GET /api/jobs/{id}/photos`
- [ ] Flutter: camera + gallery picker, upload progress
- [ ] Angular job detail: photo gallery (read-only)

## Done criteria

- Photo attached to job from mobile
- Visible on web job detail

## Next chunk

[chunk-19-parts-master.md](chunk-19-parts-master.md)
